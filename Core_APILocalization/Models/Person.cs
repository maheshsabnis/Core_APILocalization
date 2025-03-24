namespace Core_APILocalization.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Occupation { get; set; } = null!;
        public int Income { get; set; }
    }
}
