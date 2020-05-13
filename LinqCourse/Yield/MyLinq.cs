using System;
using System.Collections.Generic;

namespace LinqCourse.Yield
{
    public static class MyLinq
    {
        public static IEnumerable<T> MyFilter<T>(this IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            // No need for a concrete List and add that to the list when we fulfilled the condition
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
        
        public static IEnumerable<T> MyFilterWithoutYield<T>(this IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            var result = new List<T>();
            
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}