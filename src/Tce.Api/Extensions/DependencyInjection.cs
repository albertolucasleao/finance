using Tce.Application.Interfaces;
using Tce.Application.UseCases;
using Tce.Infrastructure.Queries;
using Tce.Infrastructure.Repositories;

namespace Tce.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // UseCases
        services.AddScoped<CreateTransactionUseCase>();
        services.AddScoped<GetTransactionsUseCase>();
        services.AddScoped<DeleteTransactionUseCase>();        
        services.AddScoped<GetTransactionByIdUseCase>();
        services.AddScoped<UpdateTransactionUseCase>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionQueryService, TransactionQueryService>();

        return services;
    }
}