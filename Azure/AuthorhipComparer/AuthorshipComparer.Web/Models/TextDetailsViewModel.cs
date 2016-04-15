namespace AuthorshipComparer.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Common;

    public class TextDetailsViewModel
    {
        public TextDetailsViewModel()
        {
            this.SimilarLinks = Enumerable.Empty<SimilarLinkViewModel>();
        }

        [DisplayName("Text Identifier")]
        public string TextId { get; set; }

        [DisplayName("Similarity Score")]
        public string SimilarityScore { get; set; }

        public string Text { get; set; }

        public IEnumerable<SimilarLinkViewModel> SimilarLinks { get; set; }

        public static TextDetailsViewModel FromEntity(TextEntity textEntity)
        {
            var viewModel = new TextDetailsViewModel
            {
                TextId = textEntity.TextId,
                SimilarityScore = textEntity.SimilarityScore.HasValue ? (textEntity.SimilarityScore.Value * 100) + "%" : "NYA"
            };

            return viewModel;
        }
    }
}
