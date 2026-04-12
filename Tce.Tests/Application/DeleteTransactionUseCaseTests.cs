using FluentAssertions;
using Moq;
using Tce.Application.Interfaces;
using Tce.Application.UseCases;
using Tce.Domain.Entities;

namespace Tce.Tests.Application;

public class DeleteTransactionUseCaseTests
{
    [Fact]
    public async Task Should_Delete_Transaction_When_Exists()
    {
        // Arrange
        var repositoryMock = new Mock<ITransactionRepository>();

        var transaction = new Transaction(
            "Test",
            100,
            Tce.Domain.Enums.TransactionType.Income,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow
        );

        repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(transaction);

        var useCase = new DeleteTransactionUseCase(repositoryMock.Object);

        // Act
        await useCase.ExecuteAsync(Guid.NewGuid());

        // Assert
        repositoryMock.Verify(x => x.DeleteAsync(transaction), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Transaction_Not_Found()
    {
        // Arrange
        var repositoryMock = new Mock<ITransactionRepository>();

        repositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Transaction?)null);

        var useCase = new DeleteTransactionUseCase(repositoryMock.Object);

        // Act
        Func<Task> act = async () => await useCase.ExecuteAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}