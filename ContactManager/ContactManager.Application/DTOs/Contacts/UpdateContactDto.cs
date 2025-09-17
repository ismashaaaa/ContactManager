namespace ContactManager.Application.DTOs.Contacts;

public record UpdateContactDto(Guid Id, string Name, DateTime DateOfBirth, bool Married, string Phone, decimal Salary);