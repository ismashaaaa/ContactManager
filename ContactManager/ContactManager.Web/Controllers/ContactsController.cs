using ContactManager.Application.DTOs.Contacts;
using ContactManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Web.Controllers;

public class ContactsController : Controller
{
    private readonly IContactService _contactService;
    private readonly ICsvService _csvService;

    public ContactsController(IContactService contactService, ICsvService csvService)
    {
        _contactService = contactService;
        _csvService = csvService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var contacts = await _contactService.GetAllContactsAsync(cancellationToken);
        return View(contacts);
    }

    public IActionResult Upload() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAsync(IFormFile csvFile, CancellationToken cancellationToken)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            ModelState.AddModelError("csvFile", "Please select a CSV file.");
            return View();
        }

        using var stream = csvFile.OpenReadStream();
        var parseResult = await _csvService.ParseCsvAsync(stream, cancellationToken);

        if (parseResult.Contacts.Any())
            await _contactService.ImportContactsAsync(parseResult.Contacts, cancellationToken);

        TempData["SuccessMessage"] = parseResult.IsSuccess
            ? $"Imported {parseResult.Contacts.Count} contacts successfully."
            : $"Imported {parseResult.Contacts.Count} contacts with {parseResult.Errors.Count} errors.";

        ViewBag.Errors = parseResult.Errors;
        return View();
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAsync(CreateContactDto createContactDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(createContactDto);

        await _contactService.AddContactAsync(createContactDto, cancellationToken);
        TempData["SuccessMessage"] = "Contact created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EditAsync(Guid id, CancellationToken cancellationToken)
    {
        var contactDto = await _contactService.GetContactByIdAsync(id, cancellationToken);
        if (contactDto == null) return RedirectToAction(nameof(Index));

        var updateContactDto = new UpdateContactDto(contactDto.Id, contactDto.Name, contactDto.DateOfBirth, contactDto.Married, contactDto.Phone, contactDto.Salary);

        return View(updateContactDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAsync(Guid id, UpdateContactDto updateContactDto, CancellationToken cancellationToken)
    {
        if (id != updateContactDto.Id || !ModelState.IsValid) return View(updateContactDto);

        await _contactService.UpdateContactAsync(updateContactDto, cancellationToken);
        TempData["SuccessMessage"] = "Contact updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _contactService.DeleteContactAsync(id, cancellationToken);
        return Json(new { success = true });
    }
}