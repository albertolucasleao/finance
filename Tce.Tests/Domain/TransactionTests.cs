using FluentAssertions;
using Tce.Domain.Entities;
using Tce.Domain.Enums;

namespace Tce.Tests.Domain;

public class TransactionTests
{
    [Fact]
    public void Should_Create_Transaction_With_Valid_Data()
    {
        // Arrange
        var description = "Test transaction";
        var amount = 100;
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var date = DateTime.UtcNow;

        // Act
        var transaction = new Transaction(
            description,
            amount,
            TransactionType.Income,
            categoryId,
            userId,
            date
        );

        // Assert
        transaction.Id.Should().NotBeEmpty();
        transaction.Description.Should().Be(description);
        transaction.Amount.Should().Be(amount);
        transaction.Type.Should().Be(TransactionType.Income);
        transaction.CategoryId.Should().Be(categoryId);
        transaction.UserId.Should().Be(userId);
        transaction.Date.Should().BeCloseTo(date, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Amount_Is_Zero()
    {
        // Arrange
        var description = "Invalid transaction";

        // Act
        Action act = () => new Transaction(
            description,
            0,
            TransactionType.Expense,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Should_Update_Transaction_With_Valid_Data()
    {
        // Arrange
        var transaction = new Transaction(
            "Old",
            100,
            TransactionType.Income,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        var newDescription = "Updated";
        var newAmount = 200;
        var newType = 1;
        var newCategoryId = Guid.NewGuid();
        var newData = DateTime.UtcNow.AddDays(1);
        var newNotes = "Detalhe da transacao";

        // Act
        transaction.Update(newDescription, newAmount, (TransactionType)newType, newCategoryId, newData, TransactionStatus.Confirmed, newNotes);

        // Assert
        transaction.Description.Should().Be(newDescription);
        transaction.Amount.Should().Be(newAmount);
        transaction.CategoryId.Should().Be(newCategoryId);
        transaction.Notes.Should().Be(newNotes);
    }

    [Fact]
    public void Should_Throw_ArgumentException_When_Updating_With_Invalid_Amount()
    {
        // Arrange
        var transaction = new Transaction(
            "Valid",
            100,
            TransactionType.Income,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        // Act
        Action act = () => transaction.Update("Test", 0, TransactionType.Expense, Guid.NewGuid(), DateTime.UtcNow, TransactionStatus.Confirmed, null);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}