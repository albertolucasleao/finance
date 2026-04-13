using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Tce.Application.DTOs;
using Tce.Application.Interfaces;

namespace Tce.Infrastructure.Queries;

public class TransactionQueryService : ITransactionQueryService
{
    private readonly string _connectionString;

    public TransactionQueryService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<SummaryDto> GetSummaryAsync(string month, Guid? categoryId = null)
    {
        using var connection = new SqlConnection(_connectionString);

        var (startDate, endDate) = ParseMonthRange(month);

        var sql = @"
            SELECT 
                COALESCE(SUM(CASE WHEN Type = 1 THEN Amount ELSE 0 END), 0) AS TotalIncome,
                COALESCE(SUM(CASE WHEN Type = 2 THEN Amount ELSE 0 END), 0) AS TotalExpense
            FROM Transactions
            WHERE Date >= @StartDate
              AND Date < @EndDate
              AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
        ";

        var result = await connection.QueryFirstAsync<SummaryDto>(sql, new
        {
            StartDate = startDate,
            EndDate = endDate,
            CategoryId = categoryId
        });

        result.Balance = result.TotalIncome - result.TotalExpense;

        return result;
    }

    public async Task<IEnumerable<ChartPointDto>> GetChartAsync(string month, Guid? categoryId = null)
    {
        using var connection = new SqlConnection(_connectionString);

        var (startDate, endDate) = ParseMonthRange(month);

        var sql = @"
            SELECT 
                DAY(Date) AS Day,
                SUM(CASE WHEN Type = 1 THEN Amount ELSE -Amount END) AS Value
            FROM Transactions
            WHERE Date >= @StartDate
              AND Date < @EndDate
              AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
            GROUP BY DAY(Date)
            ORDER BY Day
        ";

        return await connection.QueryAsync<ChartPointDto>(sql, new
        {
            StartDate = startDate,
            EndDate = endDate,
            CategoryId = categoryId
        });
    }

    public async Task<IEnumerable<TransactionHistoryDto>> GetHistoryAsync(Guid transactionId)
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = @"
        SELECT 
            Id,
            TransactionId,
            ChangedBy,
            FieldName,
            OldValue,
            NewValue,
            ChangeType,
            ChangedAt
        FROM TransactionHistories
        WHERE TransactionId = @TransactionId
        ORDER BY ChangedAt DESC
    ";

        return await connection.QueryAsync<TransactionHistoryDto>(sql, new
        {
            TransactionId = transactionId
        });
    }

    public async Task<IEnumerable<CategoryBreakdownDto>> GetCategoryBreakdownAsync(string month)
    {
        using var connection = new SqlConnection(_connectionString);

        var (startDate, endDate) = ParseMonthRange(month);

        var sql = @"
            SELECT 
                COALESCE(c.Name, 'Sem categoria') AS CategoryName,
                SUM(t.Amount) AS Total,
                COUNT(*) AS Count
            FROM Transactions t
            LEFT JOIN Categories c ON t.CategoryId = c.Id
            WHERE t.Date >= @StartDate
              AND t.Date < @EndDate
            GROUP BY c.Name
            ORDER BY Total DESC
        ";

        return await connection.QueryAsync<CategoryBreakdownDto>(sql, new
        {
            StartDate = startDate,
            EndDate = endDate
        });
    }

    private static (DateTime StartDate, DateTime EndDate) ParseMonthRange(string month)
    {
        if (!DateTime.TryParseExact($"{month}-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            throw new InvalidOperationException("Mes invalido. Use o formato yyyy-MM.");

        var startDate = new DateTime(parsedDate.Year, parsedDate.Month, 1);
        return (startDate, startDate.AddMonths(1));
    }
}