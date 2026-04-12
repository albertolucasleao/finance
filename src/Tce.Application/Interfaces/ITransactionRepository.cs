using Tce.Domain.Entities;

namespace Tce.Application.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    Task<Transaction?> GetByIdAsync(Guid id);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetPagedAsync(int page, int limit, string? month, Guid? categoryId, CancellationToken ct);
    Task<int> CountAsync(string? month, Guid? categoryId, CancellationToken ct);
}