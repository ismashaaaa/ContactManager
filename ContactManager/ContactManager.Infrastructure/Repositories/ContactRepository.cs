using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContactManager.Application.Interfaces;
using ContactManager.Domain.Entities;
using ContactManager.Infrastructure.Data;
using ContactManager.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ContactManagerDbContext _dbContext;
    private readonly IMapper _mapper;

    public ContactRepository(ContactManagerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Contact> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var contact = await _dbContext.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(contact => contact.Id == id, cancellationToken);

        return _mapper.Map<Contact>(contact);
    }

    public async Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Contacts
               .ProjectTo<Contact>(_mapper.ConfigurationProvider)
               .AsNoTracking()
               .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Contact contact, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ContactEntity>(contact);

        await _dbContext.Contacts.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken)
    {
        var contactEntities = _mapper.Map<IEnumerable<ContactEntity>>(contacts);

        await _dbContext.Contacts.AddRangeAsync(contactEntities, cancellationToken);
    }

    public async Task UpdateAsync(Contact contact, CancellationToken cancellationToken)
    {
        var existingContact = await _dbContext.Contacts
            .FindAsync(contact.Id, cancellationToken);

        if (existingContact == null)
            return;

        _mapper.Map(contact, existingContact);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingContact = await _dbContext.Contacts
            .FindAsync(id, cancellationToken);

        if (existingContact == null)
            return;

        _dbContext.Contacts.Remove(existingContact);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}