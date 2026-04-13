using Tce.Application.DTOs;
using Tce.Application.Interfaces;
using Tce.Application.UseCases;
using static System.Net.Mime.MediaTypeNames;

namespace Tce.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions");

        // POST
        group.MapPost("/", async (CreateTransactionDto dto, CreateTransactionUseCase useCase, CancellationToken ct) =>
        {
            try
            {
                var id = await useCase.ExecuteAsync(dto, ct);
                return Results.Created($"/api/transactions/{id}", id);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }

        });

        // GET LIST
        group.MapGet("/", async (int page, int limit, string? month, Guid? categoryId, GetTransactionsUseCase useCase, CancellationToken ct) =>
        {
            try
            {
                var result = await useCase.ExecuteAsync(page, limit, month, categoryId, ct);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }

        });

        // GET BY ID
        group.MapGet("/{id:guid}", async (Guid id, GetTransactionByIdUseCase useCase) =>
        {
            var result = await useCase.ExecuteAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        // PATCH
        group.MapPatch("/{id:guid}", async (Guid id, UpdateTransactionDto dto, UpdateTransactionUseCase useCase) =>
        {
            try
            {
                await useCase.ExecuteAsync(id, dto);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        // DELETE
        group.MapDelete("/{id:guid}", async (Guid id, DeleteTransactionUseCase useCase) =>
        {
            try
            {
                await useCase.ExecuteAsync(id);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
            
        });

        // PUT
        group.MapPut("/{id}", async (Guid id, UpdateTransactionDto dto, UpdateTransactionUseCase useCase) =>
        {
            try
            {
                await useCase.ExecuteAsync(id, dto);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        // SUMMARY
        group.MapGet("/summary", async (string month, Guid? categoryId, ITransactionQueryService query) =>
        {
            try
            {
                var result = await query.GetSummaryAsync(month, categoryId);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        // CHART
        group.MapGet("/chart", async (string month, Guid? categoryId, ITransactionQueryService query) =>
        {
            try
            {
                var result = await query.GetChartAsync(month, categoryId);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });

        // HISTORY
        group.MapGet("/{id:guid}/history", async (Guid id, ITransactionQueryService query) =>
        {
            var result = await query.GetHistoryAsync(id);
            return Results.Ok(result);
        });

        // CHART BY CATEGORY
        group.MapGet("/summary/by-category", async (string month, ITransactionQueryService query) =>
        {
            try
            {
                var result = await query.GetCategoryBreakdownAsync(month);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.ToString());
            }
        });
    }
}