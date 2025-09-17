using ContactManager.Application.DTOs.Csv;

namespace ContactManager.Application.Interfaces;

public interface ICsvService
{
    Task<CsvParseResult> ParseCsvAsync(Stream csvStream, CancellationToken cancellationToken);
}