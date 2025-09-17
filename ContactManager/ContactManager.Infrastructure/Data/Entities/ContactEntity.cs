namespace ContactManager.Infrastructure.Data.Entities;

public class ContactEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime DateOfBirth { get; set; }

    public bool Married { get; set; }

    public string Phone { get; set; }

    public decimal Salary { get; set; }
}