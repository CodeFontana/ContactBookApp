using ContactBook.Models;
using System.Collections.Generic;

namespace ContactBook.Services;

public class MockDataService : IContactDataService
{
    private IEnumerable<Contact> _contacts;

    public MockDataService()
    {
        _contacts = new List<Contact>()
        {
            new Contact
            {
                Name = "Brian Fontana",
                PhoneNumbers = new string[]
                {
                    "631-838-6017"
                },
                Emails = new string[]
                {
                    "brian@codefoxtrot.com",
                    "bjfontana@outlook.com",
                    "fonbr01@gmail.com"
                },
                Locations = new string[]
                {
                    "132 Bernstein Blvd"
                }
            },
            new Contact
            {
                Name = "Judy Magloczki",
                PhoneNumbers = new string[]
                {
                    "773-771-7465"
                },
                Emails = new string[]
                {
                    "judy@outeasthealth.com"
                },
                Locations = new string[]
                {
                    "132 Bernstein Blvd"
                }
            }
        };
    }

    public IEnumerable<Contact> GetContacts()
    {
        return _contacts;
    }

    public void Save(IEnumerable<Contact> contacts)
    {
        _contacts = contacts;
    }
}
