namespace AuthorshipComparer.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Common;
    using Helpers;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using Models;

    public class HomeController : Controller
    {
        private readonly CloudBlobContainer blobContainer;
        private readonly CloudQueue queue;
        private readonly CloudTable similarLinksTable;
        private readonly CloudTable textsTable;

        public HomeController()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            this.blobContainer = storageAccount.CreateCloudBlobClient().GetContainerReference("texts");
            this.blobContainer.CreateIfNotExists();
            this.queue = storageAccount.CreateCloudQueueClient().GetQueueReference("texts");
            this.queue.CreateIfNotExists();
            this.textsTable = storageAccount.CreateCloudTableClient().GetTableReference("texts");
            this.textsTable.CreateIfNotExists();
            this.similarLinksTable = storageAccount.CreateCloudTableClient().GetTableReference("similarLinks");
            this.similarLinksTable.CreateIfNotExists();
        }

        public ActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        public ActionResult EnterText()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EnterText(TextInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                return this.View();
            }

            var blobName = Guid.NewGuid().ToString();
            var blob = this.blobContainer.GetBlockBlobReference(blobName);
            blob.UploadText(model.Text.Trim());
            var textEntity = new TextEntity(this.User.Identity.GetUserId(), blobName);
            this.textsTable.Execute(TableOperation.InsertOrReplace(textEntity));
            this.queue.AddMessage(new CloudQueueMessage(blobName));

            return this.RedirectToAction("MyTexts");
        }

        [Authorize]
        public ActionResult MyTexts()
        {
            var textQuery = new TableQuery<TextEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this.User.GetId()));
            var textEntities = this.textsTable
                .ExecuteQuery(textQuery)
                .Select(TextViewModel.FromEntity);

            return this.View(textEntities);
        }

        [Authorize]
        public ActionResult Details(string id)
        {
            var textQuery = new TableQuery<TextEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id));
            var textEntity = this.textsTable
                .ExecuteQuery(textQuery)
                .Select(TextDetailsViewModel.FromEntity)
                .FirstOrDefault();

            if (textEntity == null)
            {
                throw new HttpException(404, "There is no such text.");
            }

            if (this.User.IsAdmin())
            {
                var similarLinksQuery = new TableQuery<SimilarLinkEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, textEntity.TextId));
                var similarLinks = this.similarLinksTable
                    .ExecuteQuery(similarLinksQuery)
                    .Select(SimilarLinkViewModel.FromEntity);
                textEntity.SimilarLinks = similarLinks.ToList();
            }

            textEntity.Text = this.blobContainer.GetBlockBlobReference(textEntity.TextId).DownloadText();

            return this.View(textEntity);
        }

        [AuthorizeAdmin]
        public ActionResult AllTexts()
        {
            var textEntities = this.textsTable.ExecuteQuery(new TableQuery<TextEntity>()).ToList();
            var viewModels = textEntities.Select(TextViewModel.FromEntity).ToList();

            for (var i = 0; i < viewModels.Count; i++)
            {
                viewModels[i].User = this.HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>().FindById(textEntities[i].UserId).UserName;
            }

            return this.View(viewModels);
        }
    }
}
