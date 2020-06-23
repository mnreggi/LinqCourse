using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace User
{
    public class Program
    {
        static void Main(string[] args)
        {
            var users = ProcessUsers("FilterOrderProjection/username.csv");
            var locations = processLocations("FilterOrderProjection/locations.csv");

            var queryUsers = from user in users
                where user.Id >= 5000
                orderby user.FirstName
                select user;

            foreach (var user in queryUsers.Take(3))
            {
                Console.WriteLine($"{user.Id} - {user.UserName}");
            }

            Console.WriteLine("----------------------------------------");

            var querySecurity = from location in locations
                orderby location.City descending, location.City ascending
                select new
                {
                    location.Country,
                    location.City
                };
            
            foreach (var user in querySecurity.Take(5))
            {
                Console.WriteLine($"{user.Country} - {user.City}");
            }

            Console.WriteLine("----------------------------------------");
            
            var queryJoin = from user in users
                join location in locations
                    on user.LocationId equals location.City
                orderby user.FirstName descending, location.City
                select new
                {
                    user.UserName,
                    user.FirstName,
                    location.Country,
                    location.City
                };

            var queryJoinExtension = users.Join(locations,
                    u => u.LocationId,
                    s => s.City,
                    (u, s) => new
                    {
                        u.UserName,
                        u.FirstName,
                        s.Country,
                        s.City
                    })
                .OrderByDescending(u => u.FirstName)
                .ThenBy(u => u.City);
            
            foreach (var user in queryJoin.Take(5))
            {
                Console.WriteLine($"{user.FirstName} - {user.City}");
            }
            
            Console.WriteLine("----------------------------------------");

            //If I want to JOIN with a composite key, we need to 'create' a new annonymus type that matches the ON keyword
            var queryJoinComposite = from user in users
                join location in locations
                    on new {user.LocationId, user.CountryCode}
                    equals
                    //Here I need to have exactly the same name to match the condition, that's why we are changing the
                    //name property to be the same as the user.
                    new {LocationId = location.City, location.CountryCode}
                orderby user.FirstName descending, location.City
                select new
                {
                    user.UserName,
                    user.FirstName,
                    location.Country,
                    location.City
                };
            
            foreach (var user in queryJoinComposite.Take(5))
            {
                Console.WriteLine($"{user.FirstName} - {user.City} - {user.Country}");
            }

            Console.WriteLine("----------------Group By ------------------------");

            var queryGroupBy = from location in locations
                group location by location.City.ToUpper() 
                    //Because a query syntax can end with a SELECT or a GROUP, we are not going to have a way to access this location
                    //underneath this last section. If we want for example order by some attribute, we need to put the collection
                    //into a variable, we can do this with the keyword into.
                    into userLocation
                orderby userLocation.Key
                select userLocation;
            
            var queryGroupByExtension = locations.GroupBy(s => s.City.ToUpper())
                                                .OrderBy(l => l.Key);
            
            //userLocation it is a Grouping of objects.
            foreach (var userLocation in queryGroupBy)
            {
                //KEY is the element that we grouped by. So in this case will contain like a bucket of users. Each bucket
                //is a location in this case. Bucket1 = CORDOBA, and inside we have all the users inside that bucket.
                Console.WriteLine($"{userLocation.Key} - {userLocation.Count()}");
                
                //To access each user, we need to iterate again against the grouped elements.
                foreach (var location in userLocation.OrderByDescending(l => l.City).Take(2))
                {
                    Console.WriteLine($"\t{location.City} - {location.Country}");
                }
            }
            
            Console.WriteLine("----------------- Group Join -----------------------");

            var queryGroupByJoin = from location in locations
                join user in users on location.City equals user.LocationId
                    into userGroup
                //Take all users and added into the locations.
                //Now userGroup is a new variable (user no longer exists - no just one user, have a group of users) but location we still able to use it.
                //Single Location and with it all the associated users. Now I can do the projection.
                orderby location.City
                select new
                {
                    Location = location,
                    User = userGroup
                };
            
            foreach (var groupJoin in queryGroupByJoin)
            {
                Console.WriteLine($"{groupJoin.Location.Country} - {groupJoin.Location.City}");
                
                foreach (var user in groupJoin.User.OrderByDescending(l => l.UserName).Take(2))
                {
                    Console.WriteLine($"\t{user.FirstName} - {user.Id}");
                }
            }
            
            //Now, what happen if we want the TOP 3 users order by ID, grouping them by Country?
            Console.WriteLine("----------------- Group Join Into -----------------------");

            var queryGroupByJoinInto = from location in locations
                join user in users on location.City equals user.LocationId
                    into userGroup
                orderby location.City
                select new
                {
                    Location = location,
                    User = userGroup
                }
                //We can put this result into another variable and group it by Country. The only condition is that the 
                //syntax query needs to end up with a GROUP BY or a SELECT.
                into result
                group result by result.Location.Country; 
            //These will give me a grouping of objects by country. Each of the objects will be the object the we created
            //in the SELECT operator above.

            var queryGroupByJoinIntoExtension = locations.GroupJoin(users,
                l => l.City,
                u => u.LocationId,
                (l, u) => new
                {
                    Location = l,
                    User = u
                }).OrderBy(l => l.Location.City)
                .GroupBy(l => l.Location.Country);
            
            foreach (var groupJoin in queryGroupByJoinInto)
            {
                //But now we have a GROUP, that's why we have the KEY.
                Console.WriteLine($"{groupJoin.Key}");
                
                //Inside each group we have a Users/Location collection. We need to get all of the Users inside of all of the objects
                //that are inside of this group and flatten them out so I can do my other operations.
                //If I use .SELECT() I will have a sequence of sequence, and I don't want that.
                //Using .SelectMany() will give me all of the Users across all of the items in the group
                //SelectMany converts a collection of collections into a single collection
                //Other example: Have a collection of Cars where each car has passengers. So we have a collection of Pasengers.
                //So .SelectMany will take this Collection of Collections and transform it into a single collection.
                //Will take the collection of the passengers in the first car, "add them to a temporary list", look into
                //the other CAR, take the passengers, add them to the temporary list, an so on. The result will be 
                //a single list of passengers.
                foreach (var user in groupJoin.SelectMany(u => u.User)
                                            .OrderByDescending(l => l.UserName)
                                            .Take(3))
                {
                    Console.WriteLine($"\t{user.FirstName} - {user.Id}");
                }
            }
            
            
            Console.WriteLine("----------------- Aggregation syntax -----------------------");

            var queryAggregation = from user in users
                group user by user.LocationId.ToUpper()
                into userGroup
                select new
                {
                    City = userGroup.Key,
                    Max = userGroup.Max(u => u.Math),
                    Min = userGroup.Min(u => u.Math),
                    Avg = userGroup.Average(u => u.Math)
                } into result
                orderby result.Max
                select result;

            var queryAggregationExtension1 = users.GroupBy(u => u.LocationId.ToUpper())
                .Select(userGroup => new
                {
                    City = userGroup.Key,
                    Max = userGroup.Max(u => u.Math),
                    Min = userGroup.Min(u => u.Math),
                    Avg = userGroup.Average(u => u.Math)
                }).OrderBy(u => u.Max);
            
            foreach (var groupJoin in queryAggregation)
            {
                Console.WriteLine($"{groupJoin.City}");
                Console.WriteLine($"\t Max: {groupJoin.Max}");
                Console.WriteLine($"\t Min: {groupJoin.Min}");
                Console.WriteLine($"\t Avg: {groupJoin.Avg}");
            }
            
            //The issue with the above syntax is that we are looping through the whole collection 3 times. Each for Max, Min and Avg.
            //So, How can we solve this issue? With extension method we can use Aggregate() to avoid this overworking.
            
            Console.WriteLine("----------------- Aggregate extension -----------------------");

            var queryAggregationExtension2 = users.GroupBy(u => u.LocationId.ToUpper())
                .Select(g =>
                {
                    var results = g.Aggregate(
                        //We need an accumulator. Our first param will be the initial state of the Accumulator
                        //Second param is a lambda expression that will receive the Acc and the Location object. And both will interact btw each other. It will be invoked once per item in the sequence
                        //Third param is the final result that have been computed.
                        //We will create another class that contains this statistics and have the initial values.
                        new UserStatistics(),
                        //Now we need the interaction/execution of the method.
                        (acc, u) => acc.Accumulate(u),
                        //Final param, will take an acc and give us a result.
                        acc => acc.Compute());

                    return new
                    {
                        Name = g.Key,
                        Max = results.Max,
                        Min = results.Min,
                        Avg = results.Avg
                    };
                });
            
            foreach (var groupJoin in queryAggregationExtension2)
            {
                Console.WriteLine($"{groupJoin.Name}");
                Console.WriteLine($"\t Max: {groupJoin.Max}");
                Console.WriteLine($"\t Min: {groupJoin.Min}");
                Console.WriteLine($"\t Avg: {groupJoin.Avg}");
            }
        }
        
        private static List<User> ProcessUsers(string path)
        {
            var query = File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .ToUsers();

            return query.ToList();
        }
        
        private static List<Location> processLocations(string path)
        {
            var query = File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .Select(l =>
                {
                    var columns = l.Split(";");
                    return new Location
                    {
                        City = columns[0],
                        Country = columns[1],
                        CountryCode = Convert.ToInt32(columns[2]),
                        Population = Convert.ToInt32(columns[3])
                    };
                });

            return query.ToList();
        }
    }

    public static class UserExtension
    {
        public static IEnumerable<User> ToUsers(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(";");
                yield return new User
                {
                    UserName = columns[0],
                    Id = Convert.ToInt32(columns[1]),
                    FirstName = columns[2],
                    LastName = columns[3],
                    LocationId = columns[4],
                    CountryCode = Convert.ToInt32(columns[5]),
                    Math = Convert.ToInt32(columns[6]),
                    History = Convert.ToInt32(columns[7]),
                    Science = Convert.ToInt32(columns[8])
                };
            }
        }
    }

    public class UserStatistics
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public double Avg { get; set; }
        private double Total { get; set; }
        private double Count { get; set; }

        public UserStatistics()
        {
            Max = Int32.MinValue;
            Min = Int32.MaxValue;
        }
        
        public UserStatistics Accumulate(User user)
        {
            Total += user.Math;
            Count++;
            Max = Math.Max(Max, user.Math);
            Min = Math.Min(Min, user.Math);
            
            return this;
        }

        public UserStatistics Compute()
        {
            Avg = Total / Count;
            return this;
        }
    }
}