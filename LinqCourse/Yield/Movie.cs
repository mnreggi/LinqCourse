using System;

namespace Queries
{
    public class Movie
    {
        public string Title { get; set; }
        
        // Changing this so we can see when this property is inspected.
        private int _year;
        public int Year
        {
            get
            {
                // We are gonna log when this property is inspected
                Console.WriteLine($"Returning {_year} for the movie {Title}");
                return _year;
            }
            set { _year = value; }
        }
        
        private int _giveMeException;
        public int GiveMeException
        {
            get
            {
                throw new Exception("This is an exception");
            }
            set { _giveMeException = value; }
        }
        
    }
}
