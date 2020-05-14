using System;
using System.Collections.Generic;

namespace LinqCourse.Yield
{
    public static class MyLinq
    {
        // Extension methods Allow us to define static methods
        // that appears to be a member of another type (anytype)
        #region Example of my custom extension method

        // This keyword will tell us to which kind of type we can use this extension method.
        // In this case this method will be available for all the strings.
        // Only the first method must have the 'this' keyword if we want it to be an extension method.
        public static int ParseToInt(this string myString)
        {
            return int.Parse(myString);
        }

        public static int MyCount<T>(this IEnumerable<T> sequence)
        {
            int count = 0;
            foreach (var item in sequence)
            {
                count += 1;
            }

            return count;
        }

        #endregion
        
        #region My First Filter

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

        #endregion

        #region My Second Filter PRO

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

        #endregion

    }
}