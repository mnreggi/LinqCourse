using System;
using System.Collections.Generic;
using System.Linq;
using Queries;

namespace LinqCourse.Yield
{
    class Program
    {
        static void Main(string[] args)
        {
            var movies = new List<Movie>
            {
                new Movie { Title="The Dark Knight", Year = 2008 },
                new Movie { Title="The Shinning", Year = 1980 },
                new Movie { Title ="The Spoilers", Year = 1942 },
                new Movie { Title = "Inception", Year = 2010 }
            };

            #region Extension Methods

            var testString = "2";
            Console.WriteLine(testString.ParseToInt());
            Console.WriteLine(testString.ParseToInt().GetType());

            Console.WriteLine("-------------------------------------");
            Console.WriteLine(movies.MyCount());

            #endregion

            #region My Filter Without Yield
            
            Console.WriteLine("-------------------------------------");
            // Here we are gonna execute our personalized extension method
            Console.WriteLine("\n<<<<<<< MyFilterWithoutYield starting >>>>>>>>");
            
            var query = movies.MyFilterWithoutYield(m => m.Year > 2000);
            
//            // Comment the above and Uncomment this query, to see what happens when we don't have a yield return/deferred execution.
//            // You will see that we are inspecting the whole list, and then taking the first element.
//            // Difference between the execution with yield return?
//            var query = movies.MyFilterWithoutYield(m => m.Year > 2000).Take(1);


            Console.WriteLine("\n<<<<<<< Foreach method starting >>>>>>>>");
            foreach (var movie in query)
            {
                Console.WriteLine(movie.Title);
            }
            Console.WriteLine("<<<<<<< Foreach method finished >>>>>>>>");
            
            // Here are the steps for the previous method
            // 1- Creates Empty list
            // 2- Populate the list with the items that are eligible for the condition
            // 3- Return the list with the elements
            // 4- Iterate through this list of elements.
            // 5- WriteLine for each of this elements.
            
            #endregion

            #region Where LINQ
            
            // Here we are gonna execute the Where extension method from LINQ
            Console.WriteLine("-------------------------------------\n");
            
            Console.WriteLine("<<<<<<< Where starting >>>>>>>>");
            var queryWhere = movies.Where(m => m.Year > 2000); // Deferred execution - See Title down below.

            Console.WriteLine("\n<<<<<<< Foreach method starting >>>>>>>>");
            foreach (var movie in queryWhere)
            {
                Console.WriteLine(movie.Title);
            }
            Console.WriteLine("<<<<<<< Foreach method finished >>>>>>>>");

            #endregion

            #region My Filter with yield

            // Here we are gonna execute our personalized extension method with YIELD return
            Console.WriteLine("-------------------------------------\n");
            
            Console.WriteLine("<<<<<<< MyFilter starting >>>>>>>>");
            var querYield = movies.MyFilter(m => m.Year > 2000);

            Console.WriteLine("\n<<<<<<< Foreach method starting >>>>>>>>");
            foreach (var movie in querYield)
            {
                Console.WriteLine(movie.Title);
            }
            Console.WriteLine("<<<<<<< Foreach method finished >>>>>>>>");

            // Here are the steps for the previous method
            // 1- When the foreach is called. At this point the query will be executed.
            // 2- It will create a Promise to the foreach that will receive an IEnumerable/IEnumerator.
            // 3- When the loop inside the Where hits the yield return, will return that element straight away to the caller.
            // 4- The outer foreach will print that element.
            // 5- Step 3 and 4 will repeat until there's no more elements.
            
            #endregion
            
            #region Deferred Execution
            // DEFERRED EXECUTION
            // Make LINQ as lazy as possible,
            // Not all the LINQ operators have this type of execution.
            // Define a Query or a Data Structure that knows what to do sometime in the future.
            // This query or Data structure will not execute until we try to see the results.
            // This will force the query to execute (Serialize, Json, dataBind, etc).
            
            
            // Here we are gonna execute our personalized extension method with YIELD return and also iterate through getEnumerator()
            // Debug this part to show
            Console.WriteLine("-------------------------------------\n");
            
            Console.WriteLine("<<<<<<< MyFilter Enumerator starting >>>>>>>>");
            var queryEnumerator = movies.MyFilter(m => m.Year > 2000);

            Console.WriteLine("\n<<<<<<< While method enumerator starting >>>>>>>>");
            var enumerator = queryEnumerator.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current?.Title);
            }
            Console.WriteLine("<<<<<<< While method enumerator finished >>>>>>>>");
            #endregion
            
            #region Deferred Execution with TAKE
            
            Console.WriteLine("-------------------------------------\n");
            
            Console.WriteLine("<<<<<<< MyFilter Enumerator with TAKE starting >>>>>>>>");
            var queryEnumeratorTake = movies.MyFilter(m => m.Year > 2000).Take(1);

            Console.WriteLine("\n<<<<<<< While method enumerator starting >>>>>>>>");
            var enumeratorTake = queryEnumeratorTake.GetEnumerator();
            while (enumeratorTake.MoveNext())
            {
                Console.WriteLine(enumeratorTake.Current?.Title);
                // Instead of going through all of the elements in the list.
                // We just TAKE the first one that matches the conditionand that's it.
            }
            Console.WriteLine("<<<<<<< While method enumerator finished >>>>>>>>");
            #endregion
            
            #region Merging Deferred and NOT Deferred
            
            Console.WriteLine("-------------------------------------\n");
            
            Console.WriteLine("<<<<<<< Operations with Count starting >>>>>>>>");
            var queryEnumeratorWithCount = movies.Where(m => m.Year > 2000);

            // This case, we are gonna see that Count() iterates through the whole list to get a number.
            // But late on, when we do the foreach to write each of the elements that meet the condition
            // We will see that is going to iterate again through the list.
            // This is because Count() is not Deferred Execution and Where it is.
            Console.WriteLine(queryEnumeratorWithCount.Count());
            
            // To solve that issue, we need to put this into a List and then do the count and the foreach.
            // This way we prevent to iterate twice.
            // Count() creates a new List of objects to get us a number of how many items has the list.
//            var queryEnumeratorWithCount = movies.Where(m => m.Year > 2000).ToList();

            
            Console.WriteLine("\n<<<<<<< Foreach method starting >>>>>>>>");
            foreach (var movie in queryEnumeratorWithCount)
            {
                Console.WriteLine(movie.Title);
            }
            Console.WriteLine("<<<<<<< Foreach method finished >>>>>>>>");

            #endregion
        }
    }
}