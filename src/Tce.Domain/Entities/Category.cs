using Tce.Domain.Enums;

namespace Tce.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TransactionType Type { get; private set; }
    public string? Color { get; private set; }
    public string? Icon { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Category() { }

    public Category(string name, TransactionType type, Guid userId, bool isDefault = false, string? color = null, string? icon = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        UserId = userId;
        IsDefault = isDefault;
        Color = color;
        Icon = icon;
        CreatedAt = DateTime.UtcNow;
    }
}