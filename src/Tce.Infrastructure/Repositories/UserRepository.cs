using Microsoft.EntityFrameworkCore;
using System.Threading;
using Tce.Application.Interfaces;
using Tce.Domain.Entities;
using Tce.Infrastructure.Data.Context;

namespace Tce.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == id, ct);
    }
}
