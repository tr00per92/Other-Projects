namespace AuthorshipComparer.Worker
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class StringComparer
    {
        public static double SimilarityCompare(this string first, string second)
        {
            var firstStringPairs = GetWordLetterPairs(first.ToUpper());
            var secondStringPairs = GetWordLetterPairs(second.ToUpper());

            var union = firstStringPairs.Count + secondStringPairs.Count;
            var intersection = firstStringPairs.Count(firstPair => secondStringPairs.Any(secondPair => firstPair == secondPair));

            return 2.0 * intersection / union;
        }

        private static IList<string> GetWordLetterPairs(string str)
        {
            var allPairs = new List<string>();
            var words = Regex.Split(str, @"\s");

            foreach (var word in words.Where(word => !string.IsNullOrEmpty(word)))
            {
                allPairs.AddRange(GetLetterPairs(word));
            }

            return allPairs;
        }

        private static IEnumerable<string> GetLetterPairs(string str)
        {
            var numPairs = str.Length - 1;
            var pairs = new string[numPairs];

            for (var i = 0; i < numPairs; i++)
            {
                pairs[i] = str.Substring(i, 2);
            }

            return pairs;
        }
    }
}
