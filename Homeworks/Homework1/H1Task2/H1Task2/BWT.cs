namespace H1Task2
{
    public static class BurrowsWheelerTransformation
    {
        /// <summary>
        /// Transform string using direct BWT
        /// </summary>
        public static (string, int) Transform(string toTransform)
        {
            var result = string.Empty;
            var suffixArray = CreateAndSortSuffixArray(toTransform + toTransform);
            suffixArray = suffixArray.Where(x => x < toTransform.Length).ToArray();
            foreach (var index in suffixArray)
            {
                if (index == 0)
                {
                    result += toTransform.Last();
                    continue;
                }

                result += toTransform[index - 1];
            }

            return (result, Array.IndexOf(suffixArray, 0));
        }

        /// <summary>
        /// Transform string using reverse BWT
        /// </summary>
        public static string ReverseTransform(string toTransform, int index)
        {
            var result = string.Empty;
            var elements = string.Join("", toTransform.Distinct().OrderBy(s => s));
            var countArray = new int[elements.Length];
            foreach (var i in toTransform)
            {
                countArray[elements.IndexOf(i)] += 1;
            }

            var charPositionsInSortArray = new int[toTransform.Length];
            for (var j = 0; j < toTransform.Length; j++)
            {
                charPositionsInSortArray[j] = countArray.Take(elements.IndexOf(toTransform[j])).Sum() +
                                              toTransform.Take(j).Count(x => x == toTransform[j]);
            }

            var currentPosition = index;
            while (result.Length != toTransform.Length)
            {
                result = toTransform[currentPosition] + result;
                currentPosition = charPositionsInSortArray[currentPosition];
            }

            return result;
        }

        /// <summary>
        /// Create and sort suffix array
        /// </summary>
        private static int[] CreateAndSortSuffixArray(string input)
        {
            var suffixArray = Enumerable.Range(0, input.Length).ToArray();
            Array.Sort(suffixArray, (i, j) => string.Compare(input[i..], input[j..], StringComparison.Ordinal));

            return suffixArray;
        }
    }
}