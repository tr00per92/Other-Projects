namespace AuthorshipComparer.Common
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class TextEntity : TableEntity
    {
        private string textId;
        private string userId;

        public TextEntity()
        {
        }

        public TextEntity(string userId, string textId)
        {
            this.UserId = userId;
            this.TextId = textId;
        }

        public double? SimilarityScore { get; set; }

        public string UserId
        {
            get { return this.userId; }
            set
            {
                this.userId = value;
                this.PartitionKey = value;
            }
        }

        public string TextId
        {
            get { return this.textId; }
            set
            {
                this.textId = value;
                this.RowKey = value;
            }
        }
    }
}
