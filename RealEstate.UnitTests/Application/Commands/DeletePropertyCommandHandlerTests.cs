using FluentAssertions;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Commands.Properties;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.UnitTests.Application.Commands;

public class DeletePropertyCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private DeletePropertyCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();

        _mockUnitOfWork.Setup(x => x.Properties).Returns(_mockPropertyRepository.Object);
        _handler = new DeletePropertyCommandHandler(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task HandleAsync_WithExistingProperty_ShouldSoftDeleteSuccessfully()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var command = new DeletePropertyCommand { Id = propertyId };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().BeTrue();
        existingProperty.DeletedAt.Should().NotBeNull();
        existingProperty.DeletedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithNonExistentProperty_ShouldReturnFalse()
    {
        var propertyId = Guid.NewGuid();
        var command = new DeletePropertyCommand { Id = propertyId };

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property)null);

        var result = await _handler.HandleAsync(command);

        result.Should().BeFalse();
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task HandleAsync_WithAlreadyDeletedProperty_ShouldStillSoftDelete()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var command = new DeletePropertyCommand { Id = propertyId };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");
        existingProperty.SoftDelete(); // Already deleted

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().BeTrue();
        existingProperty.DeletedAt.Should().NotBeNull();
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WhenSaveChangesFails_ShouldReturnFalse()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var command = new DeletePropertyCommand { Id = propertyId };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0); // No changes saved

        var result = await _handler.HandleAsync(command);

        result.Should().BeFalse();
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithPropertyWithImages_ShouldDeleteSuccessfully()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var command = new DeletePropertyCommand { Id = propertyId };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");
        
        // Add some images to the property
        existingProperty.AddImage("https://example.com/image1.jpg", StorageProvider.S3, "Image 1", true, 1);
        existingProperty.AddImage("https://example.com/image2.jpg", StorageProvider.S3, "Image 2", false, 2);

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().BeTrue();
        existingProperty.DeletedAt.Should().NotBeNull();
        existingProperty.Images.Should().HaveCount(2); // Images should still exist
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}