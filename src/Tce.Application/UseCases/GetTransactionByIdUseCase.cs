using Tce.Application.DTOs;
using Tce.Application.Interfaces;

namespace Tce.Application.UseCases;

public class GetTransactionByIdUseCase
{
    private readonly ITransactionRepository _repository;

    public GetTransactionByIdUseCase(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<TransactionDto?> ExecuteAsync(Guid id)
    {
        var transaction = await _repository.GetByIdAsync(id);
        if (transaction is null) return null;

        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            CategoryId = transaction.CategoryId,
            Description = transaction.Description,
            Amount = transaction.Amount,
            Type = (int)transaction.Type,
            Date = transaction.Date,
            Status = (int)transaction.Status,
            Notes = transaction.Notes,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}