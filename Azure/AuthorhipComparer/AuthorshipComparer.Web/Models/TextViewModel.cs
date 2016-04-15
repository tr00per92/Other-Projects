namespace AuthorshipComparer.Web.Models
{
    using System.ComponentModel;
    using Common;

    public class TextViewModel
    {
        [DisplayName("Text Identifier")]
        public string TextId { get; set; }

        [DisplayName("Similarity Score")]
        public string SimilarityScore { get; set; }

        public string User { get; set; }

        public static TextViewModel FromEntity(TextEntity textEntity)
        {
            var viewModel = new TextViewModel
            {
                TextId = textEntity.TextId,
                SimilarityScore = textEntity.SimilarityScore.HasValue ? (textEntity.SimilarityScore.Value * 100) + "%" : "NYA"
            };

            return viewModel;
        }
    }
}
