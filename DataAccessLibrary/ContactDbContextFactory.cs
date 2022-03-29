using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary;

public class ContactDbContextFactory
{
    private readonly string _connectionString;

    public ContactDbContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ContactDbContext CreateDbContext()
    {
        DbContextOptionsBuilder<ContactDbContext> options = new();
        options.UseSqlite(_connectionString);
        return new ContactDbContext(options.Options);
    }
}
