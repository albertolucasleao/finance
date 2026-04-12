using Tce.Application.DTOs;

namespace Tce.Application.Interfaces;

public interface ITransactionQueryService
{
    Task<SummaryDto> GetSummaryAsync(string month);
    Task<IEnumerable<ChartPointDto>> GetChartAsync(string month);
    Task<IEnumerable<TransactionHistoryDto>> GetHistoryAsync(Guid transactionId);
}