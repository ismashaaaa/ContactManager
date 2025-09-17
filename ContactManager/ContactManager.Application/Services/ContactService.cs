using AutoMapper;
using ContactManager.Application.DTOs.Contacts;
using ContactManager.Application.Interfaces;
using ContactManager.Domain.Entities;
using ContactManager.Domain.Exceptions;

namespace ContactManager.Application.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IMapper _mapper;

    public ContactService(IContactRepository contactRepository,IMapper mapper)
    {
        _contactRepository = contactRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContactDto>> GetAllContactsAsync(CancellationToken cancellationToken)
    {
        var contacts = await _contactRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<ContactDto>>(contacts);
    }

    public async Task<ContactDto?> GetContactByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var contact = await EnsureContactExistsAsync(id, cancellationToken);

        return _mapper.Map<ContactDto>(contact);
    }

    public async Task ImportContactsAsync(IEnumerable<CreateContactDto> contacts, CancellationToken cancellationToken)
    {
        var contactEntities = _mapper.Map<IEnumerable<Contact>>(contacts);

        await _contactRepository.AddRangeAsync(contactEntities, cancellationToken);
        await _contactRepository.SaveChangesAsync(cancellationToken);
    }


    public async Task AddContactAsync(CreateContactDto createContactDto, CancellationToken cancellationToken)
    {
        var contact = _mapper.Map<Contact>(createContactDto);

        await _contactRepository.AddAsync(contact, cancellationToken);
        await _contactRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateContactAsync(UpdateContactDto updateContactDto, CancellationToken cancellationToken)
    {
        var existingContact = await EnsureContactExistsAsync(updateContactDto.Id, cancellationToken);

        _mapper.Map(updateContactDto, existingContact);

        await _contactRepository.UpdateAsync(existingContact, cancellationToken);
        await _contactRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteContactAsync(Guid id, CancellationToken cancellationToken)
    {
        await EnsureContactExistsAsync(id, cancellationToken);

        await _contactRepository.DeleteAsync(id, cancellationToken);
        await _contactRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Contact> EnsureContactExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(id, cancellationToken);
        if (contact == null)
        {
            throw new EntityNotFoundException($"Contact with ID {id} not found.");
        }

        return contact;
    }
}
