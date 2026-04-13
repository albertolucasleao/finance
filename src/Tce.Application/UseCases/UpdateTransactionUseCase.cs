using Tce.Application.DTOs;
using Tce.Application.Interfaces;
using Tce.Domain.Enums;

namespace Tce.Application.UseCases;

public class UpdateTransactionUseCase
{
    private const int NotesMaxLength = 1000;

    private readonly ITransactionRepository _repository;

    public UpdateTransactionUseCase(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid id, UpdateTransactionDto dto)
    {
        var normalizedNotes = NormalizeNotes(dto.Notes);

        if (normalizedNotes is not null && normalizedNotes.Length > NotesMaxLength)
            throw new InvalidOperationException($"Notas devem ter no maximo {NotesMaxLength} caracteres.");

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
            normalizedNotes);

        await _repository.UpdateAsync(transaction);
    }

    private static string? NormalizeNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            return null;

        return notes.Trim();
    }
}