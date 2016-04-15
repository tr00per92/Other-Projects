namespace AuthorshipComparer.Web.Models
{
    using System.ComponentModel;
    using Common;

    public class SimilarLinkViewModel
    {
        public string Url { get; set; }

        [DisplayName("Similarity Score")]
        public double SimilarityScore { get; set; }

        public static SimilarLinkViewModel FromEntity(SimilarLinkEntity entity)
        {
            var viewModel = new SimilarLinkViewModel
            {
                Url = entity.Url,
                SimilarityScore = entity.SimilarityScore
            };

            return viewModel;
        }
    }
}
