namespace User
{
    public class User
    {
        public string UserName { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LocationId { get; set; }
        public int CountryCode { get; set; }
        public int Math { get; set; }
        public int History { get; set; }
        public int Science { get; set; }
    }

    public class Location
    {
        public string City { get; set; }
        public string Country { get; set; }
        public int CountryCode { get; set; }
        public int Population { get; set; }
        
    }
}