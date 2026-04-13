using FluentAssertions;
using Moq;
using Tce.Application.DTOs;
using Tce.Application.Interfaces;
using Tce.Application.UseCases;
using Tce.Domain.Entities;
using Tce.Domain.Enums;

namespace Tce.Tests.Application;

public class UpdateTransactionUseCaseTests
{
    [Fact]
    public async Task Should_Update_Transaction_When_Valid()
    {
        // Arrange
        var repositoryMock = new Mock<ITransactionRepository>();

        var transaction = new Transaction(
            "Old",
            100,
            TransactionType.Income,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(transaction);

        var useCase = new UpdateTransactionUseCase(repositoryMock.Object);

        var dto = new UpdateTransactionDto
        {
            Description = "Updated",
            Amount = 200,
            Type = (int)TransactionType.Expense,
            CategoryId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Notes = "  nota atualizada  "
        };

        // Act
        await useCase.ExecuteAsync(Guid.NewGuid(), dto);

        // Assert
        transaction.Description.Should().Be(dto.Description);
        transaction.Amount.Should().Be(dto.Amount);
        transaction.CategoryId.Should().Be(dto.CategoryId);
        transaction.Notes.Should().Be("nota atualizada");

        repositoryMock.Verify(x => x.UpdateAsync(transaction), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Transaction_Not_Found()
    {
        // Arrange
        var repositoryMock = new Mock<ITransactionRepository>();

        repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Transaction?)null);

        var useCase = new UpdateTransactionUseCase(repositoryMock.Object);

        var dto = new UpdateTransactionDto();

        // Act
        Func<Task> act = async () => await useCase.ExecuteAsync(Guid.NewGuid(), dto);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Should_Throw_InvalidOperationException_When_Updating_With_Notes_Above_Limit()
    {
        var repositoryMock = new Mock<ITransactionRepository>();

        var transaction = new Transaction(
            "Old",
            100,
            TransactionType.Income,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(transaction);

        var useCase = new UpdateTransactionUseCase(repositoryMock.Object);

        var dto = new UpdateTransactionDto
        {
            Description = "Updated",
            Amount = 200,
            Type = (int)TransactionType.Expense,
            CategoryId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Notes = new string('n', 1001)
        };

        Func<Task> act = async () => await useCase.ExecuteAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}