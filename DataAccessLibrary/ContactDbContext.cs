using DataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<Person> person = modelBuilder.Entity<Person>();
        person.HasMany(p => p.Addresses);
        person.HasMany(p => p.EmailAddresses);
        person.HasMany(p => p.PhoneNumbers);
    }
}
