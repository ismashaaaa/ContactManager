using ContactManager.Application.DTOs.Contacts;
using ContactManager.Application.DTOs.Csv;
using ContactManager.Application.Interfaces;
using System.Globalization;
using System.Text;

namespace ContactManager.Application.Services;

public class CsvService : ICsvService
{
    private readonly string[] _dateFormats = ["yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "yyyy/MM/dd", "dd.MM.yyyy"];

    public async Task<CsvParseResult> ParseCsvAsync(Stream csvStream, CancellationToken cancellationToken)
    {
        var contacts = new List<CreateContactDto>();
        var errors = new List<string>();

        int lineNumber = 0;

        try
        {
            var lines = await ReadCsvLinesAsync(csvStream, cancellationToken);

            foreach (var line in lines)
            {
                lineNumber++;
                var parseResult = ParseCsvLine(line, lineNumber);

                contacts.AddRange(parseResult.Contacts);
                errors.AddRange(parseResult.Errors);
            }

            return errors.Any()
               ? CreateFailureResult(errors, contacts)
               : CreateSuccessResult(contacts);
        }
        catch (Exception exception)
        {
            errors.Add($"Error reading CSV file: {exception.Message}");
            return CreateFailureResult(errors, contacts);
        }
    }

    private async Task<IEnumerable<string>> ReadCsvLinesAsync(Stream csvStream, CancellationToken cancellationToken)
    {
        var lines = new List<string>();
        using var reader = new StreamReader(csvStream, Encoding.UTF8);

        string line;
        bool isFirstLine = true;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (isFirstLine)
            {
                isFirstLine = false;
                if (IsHeaderLine(line)) continue;
            }

            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line);
        }

        return lines;
    }

    private bool IsHeaderLine(string line)
    {
        var normalized = line.Replace(" ", "").ToLower();
        return normalized.Contains("name") &&
               normalized.Contains("dateofbirth") &&
               normalized.Contains("married") &&
               normalized.Contains("phone") &&
               normalized.Contains("salary");
    }

    private CsvParseResult ParseCsvLine(string csvLine, int lineNumber)
    {
        var errors = new List<string>();
        var fields = ParseCsvFields(csvLine);

        if (fields.Length < 5)
            return CreateFailureResult(new[] { $"Line {lineNumber}: Expected 5 fields, found {fields.Length}" });

        var name = fields[0].Trim('"').Trim();
        if (string.IsNullOrWhiteSpace(name)) errors.Add($"Line {lineNumber}: Name cannot be empty");

        var dateText = fields[1].Trim('"').Trim();
        if (!DateTime.TryParseExact(dateText, _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
            errors.Add($"Line {lineNumber}: Invalid date format '{dateText}'");

        var marriedText = fields[2].Trim('"').Trim().ToLower();
        var married = ParseBooleanField(marriedText);
        if (married == null) errors.Add($"Line {lineNumber}: Invalid married field '{fields[2]}'");

        var phone = fields[3].Trim('"').Trim();
        if (string.IsNullOrWhiteSpace(phone)) errors.Add($"Line {lineNumber}: Phone cannot be empty");

        var salaryText = fields[4].Trim('"').Trim();
        if (!decimal.TryParse(salaryText, NumberStyles.Number | NumberStyles.Currency, CultureInfo.InvariantCulture, out var salary))
            errors.Add($"Line {lineNumber}: Invalid salary format '{salaryText}'");

        if (errors.Any())
            return CreateFailureResult(errors);

        var contact = new CreateContactDto(name, dateOfBirth, married!.Value, phone, salary);
        return CreateSuccessResult(new[] { contact });
    }

    private bool? ParseBooleanField(string value)
    {
        return value switch
        {
            "true" or "1" or "yes" or "married" => true,
            "false" or "0" or "no" or "single" => false,
            _ => null
        };
    }

    private string[] ParseCsvFields(string csvLine)
    {
        var parsedFields = new List<string>();
        var fieldBuilder = new StringBuilder();
        bool insideQuotedField = false;

        for (int charIndex = 0; charIndex < csvLine.Length; charIndex++)
        {
            char character = csvLine[charIndex];

            if (character == '"')
            {
                if (insideQuotedField && charIndex + 1 < csvLine.Length && csvLine[charIndex + 1] == '"')
                {
                    fieldBuilder.Append('"');
                    charIndex++;
                }
                else
                {
                    insideQuotedField = !insideQuotedField;
                }
            }
            else if (character == ',' && !insideQuotedField)
            {
                parsedFields.Add(fieldBuilder.ToString());
                fieldBuilder.Clear();
            }
            else
            {
                fieldBuilder.Append(character);
            }
        }

        parsedFields.Add(fieldBuilder.ToString());
        return parsedFields.ToArray();
    }

    private CsvParseResult CreateFailureResult(IEnumerable<string> errors, IEnumerable<CreateContactDto>? contacts = null)
    {
        return new CsvParseResult
        {
            IsSuccess = false,
            Contacts = contacts?.ToList() ?? new List<CreateContactDto>(),
            Errors = errors.ToList()
        };
    }

    private CsvParseResult CreateSuccessResult(IEnumerable<CreateContactDto> contacts)
    {
        return new CsvParseResult
        {
            IsSuccess = true,
            Contacts = contacts.ToList(),
            Errors = new List<string>()
        };
    }
}