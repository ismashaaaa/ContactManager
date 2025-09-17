using ContactManager.Application.DTOs.Contacts;

namespace ContactManager.Application.DTOs.Csv;

public class CsvParseResult
{
    public bool IsSuccess { get; init; }

    public List<CreateContactDto> Contacts { get; init; } = new();

    public List<string> Errors { get; init; } = new();
}