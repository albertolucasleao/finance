using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Tce.Application.DTOs;
using Tce.Application.Interfaces;
using Tce.Application.UseCases;
using Tce.Domain.Entities;
using Tce.Tests.Common;

namespace Tce.Tests.Application;

public class CreateTransactionUseCaseTests
{
    [Theory, AutoMoqData]
    public async Task Should_Create_Transaction_And_Call_Repository(
        CreateTransactionDto dto,
        [Frozen] Mock<ICategoryRepository> categoryMock,
        [Frozen] Mock<IUserRepository> userMock,
        [Frozen] Mock<ITransactionRepository> repositoryMock,
        CreateTransactionUseCase useCase)
    {
        // Arrange
        dto.Amount = 100;
        dto.Description = "Valid transaction";
        categoryMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        userMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        Tce.Domain.Entities.Transaction? captured = null;

        repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .Callback<Transaction, CancellationToken>((t, _) => captured = t);

        // Act
        var result = await useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeEmpty();

        captured.Should().NotBeNull();
        captured!.Description.Should().Be(dto.Description);
        captured.Amount.Should().Be(dto.Amount);

        repositoryMock.Verify(x =>
            x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task Should_Normalize_Notes_When_Creating_Transaction(
        CreateTransactionDto dto,
        [Frozen] Mock<ICategoryRepository> categoryMock,
        [Frozen] Mock<IUserRepository> userMock,
        [Frozen] Mock<ITransactionRepository> repositoryMock,
        CreateTransactionUseCase useCase)
    {
        dto.Amount = 120;
        dto.Description = "Transaction with notes";
        dto.Notes = "   observacao interna   ";

        categoryMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        userMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        Transaction? captured = null;
        repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .Callback<Transaction, CancellationToken>((t, _) => captured = t);

        await useCase.ExecuteAsync(dto);

        captured.Should().NotBeNull();
        captured!.Notes.Should().Be("observacao interna");
    }

    [Theory, AutoMoqData]
    public async Task Should_Throw_InvalidOperationException_When_Notes_Exceed_Max_Length(
        CreateTransactionDto dto,
        [Frozen] Mock<ICategoryRepository> categoryMock,
        [Frozen] Mock<IUserRepository> userMock,
        CreateTransactionUseCase useCase)
    {
        dto.Amount = 120;
        dto.Description = "Transaction with long notes";
        dto.Notes = new string('a', 1001);

        categoryMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        userMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        Func<Task> act = async () => await useCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory, AutoMoqData]
    public async Task Should_Throw_Exception_When_Invalid_Data(
        CreateTransactionDto dto,
        [Frozen] Mock<ICategoryRepository> categoryMock,
        [Frozen] Mock<IUserRepository> userMock,
        CreateTransactionUseCase useCase)
    {
        // Arrange
        dto.Amount = 0;
        categoryMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        userMock.Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}