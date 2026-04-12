using Tce.Application.DTOs;
using Tce.Application.Interfaces;
using Tce.Domain.Enums;

namespace Tce.Application.UseCases;

public class UpdateTransactionUseCase
{
    private readonly ITransactionRepository _repository;

    public UpdateTransactionUseCase(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid id, UpdateTransactionDto dto)
    {
        var transaction = await _repository.GetByIdAsync(id);

        if (transaction is null)
            throw new Exception("Transaction not found");

        transaction.Update(
            dto.Description,
            dto.Amount,
            (TransactionType)dto.Type,
            dto.CategoryId,
            dto.Date,
            TransactionStatus.Confirmed, // ou dto.Status se existir
            dto.Notes);

        await _repository.UpdateAsync(transaction);
    }
}