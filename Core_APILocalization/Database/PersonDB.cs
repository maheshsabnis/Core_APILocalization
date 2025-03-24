using Core_APILocalization.Models;

namespace Core_APILocalization.Database
{
    public class PersonDB : List<Person>
    {
        public PersonDB()
        {
            Add(new Person { PersonId = 101, PersonName = "Mahesh", Address = "S.B.Road", City = "Pune", Occupation = "Service", Income = 780000 });
            Add(new Person { PersonId = 102, PersonName = "Tejas", Address = "P.B.Road", City = "Pune", Occupation = "Service", Income = 670000 });
        }
    }
}
