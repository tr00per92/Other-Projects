namespace AuthorshipComparer.Common
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public class SimilarLinkEntity : TableEntity
    {
        private string textId;

        public SimilarLinkEntity()
        {
            this.RowKey = Guid.NewGuid().ToString();
        }

        public SimilarLinkEntity(string textId, string url, string userId, double similarityScore)
            : this()
        {
            this.TextId = textId;
            this.Url = url;
            this.UserId = userId;
            this.SimilarityScore = similarityScore;
        }

        public string Url { get; set; }

        public string UserId { get; set; }

        public double SimilarityScore { get; set; }

        public string TextId
        {
            get { return this.textId; }
            set
            {
                this.textId = value;
                this.PartitionKey = value;
            }
        }
    }
}
