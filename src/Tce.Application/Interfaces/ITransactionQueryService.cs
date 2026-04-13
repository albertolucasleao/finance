using Tce.Application.DTOs;

namespace Tce.Application.Interfaces;

public interface ITransactionQueryService
{
    Task<SummaryDto> GetSummaryAsync(string month, Guid? categoryId = null);
    Task<IEnumerable<ChartPointDto>> GetChartAsync(string month, Guid? categoryId = null);
    Task<IEnumerable<TransactionHistoryDto>> GetHistoryAsync(Guid transactionId);
    Task<IEnumerable<CategoryBreakdownDto>> GetCategoryBreakdownAsync(string month);
}