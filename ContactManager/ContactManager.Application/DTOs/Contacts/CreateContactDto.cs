namespace ContactManager.Application.DTOs.Contacts;

public record CreateContactDto(string Name, DateTime DateOfBirth, bool Married, string Phone, decimal Salary);
