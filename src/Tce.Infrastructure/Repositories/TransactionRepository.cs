using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Tce.Application.Interfaces;
using Tce.Domain.Entities;
using Tce.Domain.Enums;
using Tce.Infrastructure.Data.Context;

namespace Tce.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await _context.Transactions.AddAsync(transaction, ct);
        await _context.SaveChangesAsync(ct);

        // Adicionar histórico de criação
        var history = new TransactionHistory(
            transaction.Id,
            transaction.UserId, // ChangedBy
            "transaction",
            null,
            "created",
            ChangeType.Created
        );

        await _context.TransactionHistories.AddAsync(history);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();

        // Adicionar histórico de atualização
        var history = new TransactionHistory(
            transaction.Id,
            transaction.UserId, // ChangedBy
            "transaction",
            null,
            "updated",
            ChangeType.Updated
        );

        await _context.TransactionHistories.AddAsync(history);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        // Adicionar histórico de exclusão
        var history = new TransactionHistory(
            transaction.Id,
            transaction.UserId, // ChangedBy
            "transaction",
            null,
            "deleted",
            ChangeType.Deleted
        );

        await _context.TransactionHistories.AddAsync(history);

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transaction>> GetPagedAsync(int page, int limit, string? month, Guid? categoryId, CancellationToken ct)
    {
        page = page <= 0 ? 1 : page;
        limit = Math.Clamp(limit, 1, 100);

        var query = _context.Transactions
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(month))
        {
            if (DateTime.TryParseExact(
                month,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedMonth))
            {
                var start = new DateTime(parsedMonth.Year, parsedMonth.Month, 1);
                var end = start.AddMonths(1);

                query = query.Where(t => t.Date >= start && t.Date < end);
            }
            else
            {
                // loga erro
            }
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        query = query
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id);

        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? month, Guid? categoryId, CancellationToken ct)
    {
        var query = _context.Transactions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(month))
        {
            var parts = month.Split('-');

            if (parts.Length == 2 &&
                int.TryParse(parts[0], out var year) &&
                int.TryParse(parts[1], out var monthValue))
            {
                var start = new DateTime(year, monthValue, 1);
                var end = start.AddMonths(1);

                query = query.Where(t =>
                    t.Date >= start &&
                    t.Date < end
                );
            }
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        return await query.CountAsync(ct);
    }
}