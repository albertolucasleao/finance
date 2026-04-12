using Tce.Application.Interfaces;

namespace Tce.Application.UseCases;

public class DeleteTransactionUseCase
{
    private readonly ITransactionRepository _repository;

    public DeleteTransactionUseCase(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        var transaction = await _repository.GetByIdAsync(id);

        if (transaction is null)
            throw new Exception("Transaction not found");

        await _repository.DeleteAsync(transaction);
    }
}