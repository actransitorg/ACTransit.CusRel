using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACTransit.Framework.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> OrderByNatural<T>(this IEnumerable<T> items, Func<T, string> selector, StringComparer stringComparer = null)
        {
            var regex = new Regex(@"\d+", RegexOptions.Compiled);

            var enumerable = items as T[] ?? items.ToArray();
            var maxDigits = enumerable
                                .SelectMany(i => regex.Matches(selector(i))
                                    .Cast<Match>()
                                    .Select(digitChunk => (int?) digitChunk.Value.Length))
                                .Max() ?? 0;

            return enumerable.OrderBy(i => regex.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')),
                stringComparer ?? StringComparer.CurrentCulture);
        }

        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> func)
        {
            var itemsArray = items as T[] ?? items.ToArray();
            var count = itemsArray.Count();
            for (var i = 0; i < count; i++)
            {
                T item = itemsArray[i];                
                if (func(item))
                    return i;
            }
            return -1;
        }


    }
}
