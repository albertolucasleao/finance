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

    public async Task<SummaryDto> GetSummaryAsync(string month)
    {
        using var connection = new SqlConnection(_connectionString);

        var date = DateTime.Parse($"{month}-01");

        var sql = @"
            SELECT 
                SUM(CASE WHEN Type = 1 THEN Amount ELSE 0 END) AS TotalIncome,
                SUM(CASE WHEN Type = 2 THEN Amount ELSE 0 END) AS TotalExpense
            FROM Transactions
            WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year
        ";

        var result = await connection.QueryFirstAsync<SummaryDto>(sql, new
        {
            Month = date.Month,
            Year = date.Year
        });

        result.Balance = result.TotalIncome - result.TotalExpense;

        return result;
    }

    public async Task<IEnumerable<ChartPointDto>> GetChartAsync(string month)
    {
        using var connection = new SqlConnection(_connectionString);

        var date = DateTime.Parse($"{month}-01");

        var sql = @"
            SELECT 
                DAY(Date) AS Day,
                SUM(CASE WHEN Type = 1 THEN Amount ELSE -Amount END) AS Value
            FROM Transactions
            WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year
            GROUP BY DAY(Date)
            ORDER BY Day
        ";

        return await connection.QueryAsync<ChartPointDto>(sql, new
        {
            Month = date.Month,
            Year = date.Year
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
}