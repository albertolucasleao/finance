namespace Tce.Application.DTOs;

public class TransactionHistoryDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public Guid ChangedBy { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int ChangeType { get; set; }
    public DateTime ChangedAt { get; set; }
}