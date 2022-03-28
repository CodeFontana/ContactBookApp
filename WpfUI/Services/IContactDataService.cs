using ContactBook.Models;
using System.Collections.Generic;

namespace ContactBook.Services;

public interface IContactDataService
{
    IEnumerable<Contact> GetContacts();
    void Save(IEnumerable<Contact> contacts);
}