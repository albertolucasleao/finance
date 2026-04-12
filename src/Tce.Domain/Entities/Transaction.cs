using Tce.Domain.Enums;

namespace Tce.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Date { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public List<TransactionHistory> Histories { get; set; } = new();

    protected Transaction() { }

    public Transaction(
        string description,
        decimal amount,
        TransactionType type,
        Guid? categoryId,
        Guid userId,
        DateTime date,
        TransactionStatus status = TransactionStatus.Confirmed,
        string? notes = null)
    {
        if (amount <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.");

        Id = Guid.NewGuid();
        Description = description;
        Amount = amount;
        Type = type;
        CategoryId = categoryId;
        UserId = userId;
        Date = date;
        Status = status;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string description, decimal amount, TransactionType type, Guid? categoryId, DateTime date, TransactionStatus status, string? notes)
    {
        if (amount <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.");

        Description = description;
        Amount = amount;
        Type = type;
        CategoryId = categoryId;
        Date = date;
        Status = status;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}