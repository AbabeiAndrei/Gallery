using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gallery.Extensions
{
    public static class LinqEx
    {
        public static IEnumerable<T> Multiply<T>(this IEnumerable<T> source, int count, bool forceClone = false)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));

            if(count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var items = source as IList<T> ?? source.ToList();

            var result = new List<T>();

            while (count-- > 0)
                result.AddRange(forceClone 
                                    ? items.Select(i => i.Clone()) 
                                    : items);

            return result;
        }

        public static IList<T> OnLast<T>(this IList<T> source, Action<T> action)
        {
            if(action == null)
                throw new ArgumentNullException(nameof(action));

            return OnLast(source, (arg, count) => action(arg));
        }

        public static IList<T> OnItem<T>(this IList<T> source, int itemIndex, Action<T, int> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            itemIndex--;

            if (source.Count > itemIndex)
            {
                var item = source[itemIndex];

                if (item != null)
                    action(item, source.Count);

                return source;
            }

            return source;
        }

        public static IList<T> OnLast<T>(this IList<T> source, Action<T, int> action)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var last = source.LastOrDefault();

            if (last != null)
                action(last, source.Count);

            return source;
        }
    }
}