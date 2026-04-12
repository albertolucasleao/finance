using Microsoft.EntityFrameworkCore;
using System.Threading;
using Tce.Application.Interfaces;
using Tce.Domain.Entities;
using Tce.Infrastructure.Data.Context;

namespace Tce.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == id, ct);
    }
}