using Tce.Application.Interfaces;

namespace Tce.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", async (ICategoryRepository repo) =>
        {
            var result = await repo.GetAllAsync();
            return Results.Ok(result);
        });
    }
}