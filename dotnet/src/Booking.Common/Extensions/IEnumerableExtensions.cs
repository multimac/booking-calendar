using System;
using System.Collections.Generic;
using System.Linq;

namespace Booking.Common.Extensions
{
    public static class IEnumerableExtensions
    {   
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;
            
            foreach (var item in source)
            {
                if(bucket == null)
                    bucket = new T[size];
                
                bucket[count++] = item;
                
                if(count != size)
                    continue;
                
                yield return bucket.Select(x => x);
                
                bucket = null;
                count = 0;
            }
            
            if(bucket != null && count > 0)
                yield return bucket.Take(count);
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Func<int, int> rng)
        {
            T[] elements = source.ToArray();
            for(int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = rng(i + 1);
                
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
        {
            foreach (var item in source)
            {
                yield return item;
                
                var children = childSelector(item);
                if(children == null)
                    continue;
                
                foreach (var child in children.Traverse(childSelector))
                    yield return child;
            }
        }
    }
}