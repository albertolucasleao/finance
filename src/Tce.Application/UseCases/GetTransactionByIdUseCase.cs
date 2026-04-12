using Tce.Application.Interfaces;

namespace Tce.Application.UseCases;

public class GetTransactionByIdUseCase
{
    private readonly ITransactionRepository _repository;

    public GetTransactionByIdUseCase(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<object?> ExecuteAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}