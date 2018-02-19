using System;
using System.Linq;
using System.Linq.Dynamic;

namespace PWET.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string sort)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (sort == null)
            {
                return source;
            }

            var lstSort = sort.Split(',');

            foreach (var sortOption in lstSort.Reverse())
            {
                if (sortOption.StartsWith("-"))
                {
                    source = source.OrderBy(sortOption.Remove(0, 1) + " descending");
                }
                else
                {
                    source = source.OrderBy(sortOption);
                }

            }
            return source;
        }
    }
}
