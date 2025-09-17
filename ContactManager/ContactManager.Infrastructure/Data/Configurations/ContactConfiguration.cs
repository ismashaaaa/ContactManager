using ContactManager.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactManager.Infrastructure.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<ContactEntity>
{
    public void Configure(EntityTypeBuilder<ContactEntity> builder)
    {
        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(contact => contact.DateOfBirth)
            .IsRequired();

        builder.Property(contact => contact.Phone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(contact => contact.Salary)
            .IsRequired()
            .HasPrecision(18, 2);
    }
}
