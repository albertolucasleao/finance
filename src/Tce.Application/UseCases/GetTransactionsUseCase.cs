using Tce.Application.DTOs;
using Tce.Application.Common;
using Tce.Application.Interfaces;

namespace Tce.Application.UseCases;

public class GetTransactionsUseCase
{
    private readonly ITransactionRepository _repository;

    public GetTransactionsUseCase(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TransactionDto>> ExecuteAsync(int page, int limit, string? month, Guid? categoryId, CancellationToken ct)
    {
        var data = await _repository.GetPagedAsync(page, limit, month, categoryId, ct);
        var total = await _repository.CountAsync(month, categoryId, ct);

        var result = data.Select(x => new TransactionDto
        {
            Id = x.Id,
            Description = x.Description,
            Amount = x.Amount,
            Type = (int)x.Type,
            CategoryId = x.CategoryId,
            UserId = x.UserId,
            Date = x.Date,
            Status = (int)x.Status,
            Notes = x.Notes,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });

        return new PagedResult<TransactionDto>
        {
            Data = result,
            Total = total
        };
    }
}