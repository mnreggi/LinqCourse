using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using User;

namespace LinqToXml
{
    class Program
    {
        static void Main(string[] args)
        {
            var records = ProcessUsers("username.csv");
//            CreateOption1(records);
//            CreateOption2(records);
//            CreateOption3(records);
            CreateOption4(records);
            LoadXml();
        }

        private static void LoadXml()
        {
            //XDocument assumes that we can load the whole file in memory. For large files, we might still need to refer
            //to XmlReader.
            var document = XDocument.Load("Users.xml");

            //To access the elements we just need to go through each of the elements in my XML.
            //Elements, will return an IEnumerable, so we can query this using LINQ.
            //The problem is if the Attribute does not exist. Will throw an Exception. We need to check before accessing the value
            var query = from element in document.Element("Users")?.Elements("User")
                where element.Attribute("Location")?.Value == "Porirua"
                select element.Attribute("Name")?.Value;
            
            //We can also use
            //from element in document.Descendants("User")
            
            foreach (var user in query)
            {
                Console.WriteLine(user);
            }
        }

        private static void CreateOption1(List<User.User> records)
        {
            var document = new XDocument();
            var users = new XElement("Users");

            foreach (var record in records)
            {
                var user = new XElement("User");
                var userName = new XAttribute("Name", record.UserName);
                var math = new XAttribute("Math", record.Math);
                
                user.Add(userName);
                user.Add(math);
                
                users.Add(user);
            }

            document.Add(users);
            document.Save("Users.xml");
        }
        
        private static void CreateOption2(List<User.User> records)
        {
            var document = new XDocument();
            var users = new XElement("Users");

            foreach (var record in records)
            {
                var user = new XElement("User", 
                    new XAttribute("Name", record.UserName),
                    new XAttribute("Math", record.Math));

                users.Add(user);
            }

            document.Add(users);
            document.Save("Users.xml");
        }
        
        private static void CreateOption3(List<User.User> records)
        {
            var document = new XDocument();
            var users = new XElement("Users");
            
            var elements = from record in records
                select new XElement("User", 
                    new XAttribute("Name", record.UserName),
                    new XAttribute("Math", record.Math));

            users.Add(elements);
            
            document.Add(users);
            document.Save("Users.xml");
        }
        
        private static void CreateOption4(List<User.User> records)
        {
            var document = new XDocument();
            var users = new XElement("Users", 
                from record in records
                select new XElement("User", 
                    new XAttribute("Name", record.UserName),
                    new XAttribute("Math", record.Math),
                    new XAttribute("Location", record.LocationId)));
            
            document.Add(users);
            document.Save("Users.xml");
        }
        
        private static List<User.User> ProcessUsers(string path)
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
        public static IEnumerable<User.User> ToUsers(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(";");
                yield return new User.User
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
}