namespace AuthorshipComparer.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Google.Apis.Customsearch.v1;
    using Google.Apis.Customsearch.v1.Data;
    using Google.Apis.Services;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;

    public class WorkerRole : RoleEntryPoint
    {
        private const double ScoreThreshold = 0.15;
        private const int MaxQueryLength = 40;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly Regex regex = new Regex(@"[^\w Р-пр-џ]", RegexOptions.Compiled);
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private CloudBlobContainer blobContainer;
        private CustomsearchService customSearchService;
        private CloudTable linksTable;
        private CloudQueue queue;
        private CloudTable textsTable;

        private WebClient webClient;

        public override void Run()
        {
            Trace.TraceInformation("AuthorshipComparer.Worker is running");
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            Trace.TraceInformation("AuthorshipComparer.Worker has been started");
            ServicePointManager.DefaultConnectionLimit = 12;

            this.customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = Settings.Default.GoogleApiKey });
            this.webClient = new WebClient { Encoding = Encoding.UTF8 };

            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            this.blobContainer = storageAccount.CreateCloudBlobClient().GetContainerReference("texts");
            this.blobContainer.CreateIfNotExists();
            this.queue = storageAccount.CreateCloudQueueClient().GetQueueReference("texts");
            this.queue.CreateIfNotExists();

            var tableClient = storageAccount.CreateCloudTableClient();
            this.linksTable = tableClient.GetTableReference("similarLinks");
            this.linksTable.CreateIfNotExists();
            this.textsTable = tableClient.GetTableReference("texts");
            this.textsTable.CreateIfNotExists();

            Trace.TraceInformation("Cloud storage initialized");
            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("AuthorshipComparer.Worker is stopping");
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
            base.OnStop();
            this.webClient.Dispose();
            Trace.TraceInformation("AuthorshipComparer.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await this.queue.GetMessageAsync(cancellationToken);
                if (message != null)
                {
                    await this.ProcessMessage(message, cancellationToken);
                    await this.queue.DeleteMessageAsync(message, cancellationToken);
                }
                else
                {
                    await Task.Delay(10000, cancellationToken);
                }
            }
        }

        private async Task ProcessMessage(CloudQueueMessage message, CancellationToken cancellationToken)
        {
            var blob = this.blobContainer.GetBlockBlobReference(message.AsString);
            var textString = await blob.DownloadTextAsync(cancellationToken);
            var textEntity = this.RetrieveTextEntity(blob.Name);
            var totalSimilarityScore = 0.0;

            var searchResults = await this.GetSearchResults(textString, cancellationToken);
            foreach (var item in searchResults)
            {
                try
                {
                    var textFromWeb = await this.webClient.DownloadStringTaskAsync(item.Link);
                    textFromWeb = this.regex.Replace(textFromWeb, string.Empty);
                    var similarityScore = textFromWeb.SimilarityCompare(textString);
                    if (similarityScore >= ScoreThreshold)
                    {
                        var linkEntity = new SimilarLinkEntity(blob.Name, item.Link, textEntity.UserId, Math.Round(similarityScore, 2));
                        this.linksTable.Execute(TableOperation.InsertOrReplace(linkEntity));
                    }

                    totalSimilarityScore += similarityScore;
                }
                catch (WebException)
                {
                }
            }

            textEntity.SimilarityScore = Math.Round(totalSimilarityScore / searchResults.Count, 2);
            await this.textsTable.ExecuteAsync(TableOperation.Merge(textEntity), cancellationToken);
        }

        private async Task<ICollection<Result>> GetSearchResults(string text, CancellationToken cancellationToken)
        {
            var searchQuery = ConstructSearchQuery(text);
            var listRequest = this.customSearchService.Cse.List(searchQuery);
            listRequest.Cx = Settings.Default.SearchEngineId;
            var searchResult = await listRequest.ExecuteAsync(cancellationToken);

            return searchResult.Items;
        }

        private TextEntity RetrieveTextEntity(string entityKey)
        {
            var textQuery = new TableQuery<TextEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, entityKey));
            var textEntity = this.textsTable.ExecuteQuery(textQuery).Single();

            return textEntity;
        }

        private static string ConstructSearchQuery(string text)
        {
            var query = text;
            if (query.Length > MaxQueryLength)
            {
                var index = text.IndexOf(" ", MaxQueryLength, StringComparison.InvariantCulture);
                if (index == -1)
                {
                    index = text.LastIndexOf(" ", 0, MaxQueryLength, StringComparison.InvariantCulture);
                }

                if (index != -1)
                {
                    query = text.Substring(0, index);
                }
            }

            return query;
        }
    }
}
