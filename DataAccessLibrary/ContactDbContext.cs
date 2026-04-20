using DataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary;

public class ContactDbContext : DbContext
{
    public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Contacts { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<Phone> Phones { get; set; }
}
