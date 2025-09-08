using FluentAssertions;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Commands.Owners;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Application.Commands;

public class DeleteOwnerCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IOwnerRepository> _mockOwnerRepository;
    private DeleteOwnerCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOwnerRepository = new Mock<IOwnerRepository>();

        _mockUnitOfWork.Setup(x => x.Owners).Returns(_mockOwnerRepository.Object);

        _handler = new DeleteOwnerCommandHandler(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task HandleAsync_WithExistingOwner_ShouldSoftDeleteSuccessfully()
    {
        var ownerId = Guid.NewGuid();
        var command = new DeleteOwnerCommand { Id = ownerId };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().BeTrue();
        existingOwner.DeletedAt.Should().NotBeNull();
        existingOwner.DeletedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithNonExistentOwner_ShouldReturnFalse()
    {
        var ownerId = Guid.NewGuid();
        var command = new DeleteOwnerCommand { Id = ownerId };

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Owner)null);

        var result = await _handler.HandleAsync(command);

        result.Should().BeFalse();
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task HandleAsync_WithAlreadyDeletedOwner_ShouldStillSoftDelete()
    {
        var ownerId = Guid.NewGuid();
        var command = new DeleteOwnerCommand { Id = ownerId };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");
        existingOwner.SoftDelete(); // Already deleted

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().BeTrue();
        existingOwner.DeletedAt.Should().NotBeNull();
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WhenSaveChangesFails_ShouldReturnFalse()
    {
        var ownerId = Guid.NewGuid();
        var command = new DeleteOwnerCommand { Id = ownerId };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0); // No changes saved

        var result = await _handler.HandleAsync(command);

        result.Should().BeFalse();
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}