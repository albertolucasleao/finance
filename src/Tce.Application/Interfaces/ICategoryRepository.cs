using Tce.Domain.Entities;

namespace Tce.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}