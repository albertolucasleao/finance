using Tce.Domain.Enums;

namespace Tce.Domain.Entities;

public class TransactionHistory
{
    public Guid Id { get; private set; }
    public Guid TransactionId { get; private set; }
    public Guid ChangedBy { get; private set; }
    public string FieldName { get; private set; } = string.Empty;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public ChangeType ChangeType { get; private set; }
    public DateTime ChangedAt { get; private set; }

    protected TransactionHistory() { }

    public TransactionHistory(Guid transactionId, Guid changedBy, string fieldName, string? oldValue, string? newValue, ChangeType changeType)
    {
        Id = Guid.NewGuid();
        TransactionId = transactionId;
        ChangedBy = changedBy;
        FieldName = fieldName;
        OldValue = oldValue;
        NewValue = newValue;
        ChangeType = changeType;
        ChangedAt = DateTime.UtcNow;
    }
}