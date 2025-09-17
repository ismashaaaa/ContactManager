using ContactManager.Application.DTOs.Contacts;

namespace ContactManager.Application.Interfaces;

public interface IContactService
{
    Task<IEnumerable<ContactDto>> GetAllContactsAsync(CancellationToken cancellationToken);

    Task<ContactDto?> GetContactByIdAsync(Guid id, CancellationToken cancellationToken);

    Task ImportContactsAsync(IEnumerable<CreateContactDto> contacts, CancellationToken cancellationToken);

    Task AddContactAsync(CreateContactDto createContactDto, CancellationToken cancellationToken);

    Task UpdateContactAsync(UpdateContactDto updateContactDto, CancellationToken cancellationToken);

    Task DeleteContactAsync(Guid id, CancellationToken cancellationToken);
}
