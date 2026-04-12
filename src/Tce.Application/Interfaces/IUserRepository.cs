using System.Threading;
using System.Threading.Tasks;
using Tce.Domain.Entities;

namespace Tce.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
