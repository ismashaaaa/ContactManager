using ContactManager.Domain.Entities;

namespace ContactManager.Application.Interfaces;

public interface IContactRepository
{
    Task<Contact> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Contact contact, CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken);

    Task UpdateAsync(Contact contact, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}