namespace ContactManager.Application.DTOs.Contacts;

public record ContactDto(Guid Id, string Name, DateTime DateOfBirth, bool Married, string Phone, decimal Salary);