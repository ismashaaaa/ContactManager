using ContactManager.Application.DTOs.Contacts;
using ContactManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsApiController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsApiController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IEnumerable<ContactDto>> GetAllContactsAsync(CancellationToken cancellationToken)
    {
        return await _contactService.GetAllContactsAsync(cancellationToken);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContactDto>> UpdateContactAsync(Guid id, UpdateContactDto updateContactDto, CancellationToken cancellationToken)
    {
        if (id != updateContactDto.Id) 
            return BadRequest("ID mismatch");

        await _contactService.UpdateContactAsync(updateContactDto, cancellationToken);
        var updatedContact = await _contactService.GetContactByIdAsync(id, cancellationToken);

        return Ok(updatedContact);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContactAsync(Guid id, CancellationToken cancellationToken)
    {
        await _contactService.DeleteContactAsync(id, cancellationToken);

        return NoContent();
    }
}