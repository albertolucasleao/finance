using Tce.Domain.Entities;
using Tce.Domain.Enums;
using Tce.Infrastructure.Data.Context;

namespace Tce.Infrastructure.Data.Seed;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (context.Users.Any()) return;
        
        // USER
        var user = new User("Alberto", "alberto@example.com");

        context.Users.Add(user);
        context.SaveChanges();

        // CATEGORIES
        var categories = new List<Category>
        {
            new("Salário", TransactionType.Income, user.Id, true),
            new("Freelance", TransactionType.Income, user.Id, true),
            new("Alimentação", TransactionType.Expense, user.Id, true),
            new("Transporte", TransactionType.Expense, user.Id, true),
            new("Lazer", TransactionType.Expense, user.Id, true)
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        // TRANSACTIONS
        var random = new Random();
        var transactions = new List<Transaction>();

        for (int i = 1; i <= 15; i++)
        {
            var isIncome = i % 3 == 0;

            var category = categories
                .Where(c => c.Type == (isIncome ? TransactionType.Income : TransactionType.Expense))
                .OrderBy(_ => Guid.NewGuid())
                .First();

            var transaction = new Transaction(
                description: isIncome ? "Entrada exemplo" : "Despesa exemplo",
                amount: random.Next(50, 500),
                type: isIncome ? TransactionType.Income : TransactionType.Expense,
                categoryId: category.Id,
                userId: user.Id,
                date: DateTime.UtcNow.AddDays(-i),
                status: TransactionStatus.Confirmed
            );

            transactions.Add(transaction);
        }

        context.Transactions.AddRange(transactions);
        context.SaveChanges();

        // HISTÓRICO
        var histories = new List<TransactionHistory>();

        foreach (var transaction in transactions)
        {
            histories.Add(new TransactionHistory(
                transaction.Id,
                user.Id,
                "transaction",
                null,
                "created",
                ChangeType.Created
            ));
        }

        context.TransactionHistories.AddRange(histories);
        context.SaveChanges();
    }
}