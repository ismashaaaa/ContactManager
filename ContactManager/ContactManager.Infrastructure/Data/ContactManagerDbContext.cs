using ContactManager.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Infrastructure.Data;

public class ContactManagerDbContext : DbContext
{
    public ContactManagerDbContext(DbContextOptions<ContactManagerDbContext> options) : base(options)
    { }

    public DbSet<ContactEntity> Contacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContactManagerDbContext).Assembly);
    }
}
