namespace CommenHelper
{
    public static class ListHelper
    {
        public static bool CompareList<T>(IEnumerable<T> list1, IEnumerable<T> list2) where T : IComparable
        {
            //list1.All(x => list2.Any(y => y.CompareTo(x) == 0));这样写也可以
            IEnumerable<T> sortedList1 = list1.OrderBy(x => x);
            IEnumerable<T> sortedList2 = list2.OrderBy(x => x);
            return sortedList1.SequenceEqual(sortedList2);
        }
    }
}
