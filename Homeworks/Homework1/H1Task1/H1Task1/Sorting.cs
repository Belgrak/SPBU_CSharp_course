namespace H1Task1
{
    public static class InsertionSort
    {
        /// <summary>
        /// Sorts the elements in the list of ints
        /// </summary>
        public static IEnumerable<int> Sort(IList<int> listToSort)
        {
            for (var i = 1; i < listToSort.Count; i++)
            {
                for (var j = i; j > 0 && listToSort[j - 1] > listToSort[j]; j--)
                {
                    (listToSort[j - 1], listToSort[j]) = (listToSort[j], listToSort[j - 1]);
                }
            }

            return listToSort;
        }
    }
}