namespace Tce.Application.DTOs;

public class CreateTransactionDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Type { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}