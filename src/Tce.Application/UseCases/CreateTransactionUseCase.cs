using Tce.Application.DTOs;
using Tce.Application.Interfaces;
using Tce.Domain.Entities;
using Tce.Domain.Enums;

namespace Tce.Application.UseCases;

public class CreateTransactionUseCase
{
    private const int NotesMaxLength = 1000;

    private readonly ITransactionRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;

    public CreateTransactionUseCase(
        ITransactionRepository repository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
    }

    public async Task<Guid> ExecuteAsync(CreateTransactionDto dto, CancellationToken ct = default)
    {
        var normalizedNotes = NormalizeNotes(dto.Notes);

        if (normalizedNotes is not null && normalizedNotes.Length > NotesMaxLength)
            throw new InvalidOperationException($"Notas devem ter no maximo {NotesMaxLength} caracteres.");

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId, ct))
            throw new InvalidOperationException("Categoria não encontrada.");

        if (!await _userRepository.ExistsAsync(dto.UserId, ct))
            throw new InvalidOperationException("Usuário não encontrado.");

        var transaction = new Transaction(
            dto.Description,
            dto.Amount,
            (TransactionType)dto.Type,
            dto.CategoryId,
            dto.UserId,
            dto.Date,
            TransactionStatus.Confirmed,
            normalizedNotes
        );

        await _repository.AddAsync(transaction, ct);

        return transaction.Id;
    }

    private static string? NormalizeNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            return null;

        return notes.Trim();
    }
}