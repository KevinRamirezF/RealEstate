using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Commands.Owners;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Application.Commands;

public class UpdateOwnerCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IOwnerRepository> _mockOwnerRepository;
    private OwnerMapper _mapper;
    private UpdateOwnerDtoValidator _validator;
    private UpdateOwnerCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOwnerRepository = new Mock<IOwnerRepository>();
        _mapper = new OwnerMapper();
        _validator = new UpdateOwnerDtoValidator();

        _mockUnitOfWork.Setup(x => x.Owners).Returns(_mockOwnerRepository.Object);

        _handler = new UpdateOwnerCommandHandler(_mockUnitOfWork.Object, _mapper, _validator);
    }

    [Test]
    public async Task HandleAsync_WithValidData_ShouldUpdateOwnerSuccessfully()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Updated",
            LastName = "Owner",
            Email = "updated@example.com",
            PhoneNumber = "+19876543210",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            AddressLine = "Updated Address",
            City = "Updated City",
            State = "NY",
            PostalCode = "10001",
            Country = "US",
            ExternalCode = "UPD001"
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var existingOwner = Owner.Create("Old Name", "old@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByEmailAsync(updateDto.Email, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockOwnerRepository.Setup(x => x.ExistsByExternalCodeAsync(updateDto.ExternalCode, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("Updated Owner");
        result.Email.Should().Be("updated@example.com");
        result.PhoneNumber.Should().Be("+19876543210");
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithNonExistentOwner_ShouldReturnNull()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Updated",
            LastName = "Owner",
            Email = "updated@example.com",
            PhoneNumber = "+19876543210",
            AddressLine = "123 Test St",
            City = "Test City",
            State = "NY", 
            PostalCode = "10001",
            Country = "US"
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Owner)null);

        var result = await _handler.HandleAsync(command);

        result.Should().BeNull();
    }

    [Test]
    public async Task HandleAsync_WithDuplicateEmail_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Test",
            LastName = "Owner",
            Email = "existing@example.com",
            PhoneNumber = "+12345678900",
            AddressLine = "123 Test St",
            City = "Test City",
            State = "NY",
            PostalCode = "12345",
            Country = "US"
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var existingOwner = Owner.Create("Test Owner", "old@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByEmailAsync(updateDto.Email, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("An owner with this email already exists.");
    }

    [Test]
    public async Task HandleAsync_WithDuplicateExternalCode_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Test",
            LastName = "Owner",
            Email = "test@example.com",
            PhoneNumber = "+12345678900",
            AddressLine = "123 Test St",
            City = "Test City",
            State = "NY",
            PostalCode = "12345",
            Country = "US",
            ExternalCode = "EXISTING_CODE"
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByExternalCodeAsync(updateDto.ExternalCode, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("An owner with this external code already exists.");
    }

    [Test]
    public async Task HandleAsync_WithSameEmailAsExisting_ShouldNotCheckDuplication()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Updated",
            LastName = "Owner",
            Email = "same@example.com", // Same as existing
            PhoneNumber = "+19876543210",
            AddressLine = "123 Test St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
            Country = "US"
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var existingOwner = Owner.Create("Test Owner", "same@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("Updated Owner");
        result.Email.Should().Be("same@example.com");
        
        // Should not check for email duplication since it's the same
        _mockOwnerRepository.Verify(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task HandleAsync_WithEmptyExternalCode_ShouldNotCheckDuplication()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Updated",
            LastName = "Owner",
            Email = "updated@example.com",
            PhoneNumber = "+19876543210",
            AddressLine = "123 Test St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
            Country = "US",
            ExternalCode = "" // Empty external code
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByEmailAsync(updateDto.Email, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("Updated Owner");
        
        // Should not check for external code duplication since it's empty
        _mockOwnerRepository.Verify(x => x.ExistsByExternalCodeAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task HandleAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "", // Empty first name
            LastName = "Owner",
            Email = "invalid-email", // Invalid email format
            PhoneNumber = "invalid123" // Invalid phone format
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HandleAsync_WithAllOptionalProperties_ShouldSetAllProperties()
    {
        var ownerId = Guid.NewGuid();
        var birthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Complete",
            LastName = "Owner",
            Email = "complete@example.com",
            PhoneNumber = "+19876543210",
            DateOfBirth = birthDate,
            AddressLine = "123 Main St",
            City = "Test City",
            State = "CA",
            PostalCode = "90210",
            Country = "US",
            ExternalCode = "EXT123"
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var existingOwner = Owner.Create("Old Name", "old@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByEmailAsync(updateDto.Email, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockOwnerRepository.Setup(x => x.ExistsByExternalCodeAsync(updateDto.ExternalCode, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("Complete Owner");
        result.Email.Should().Be("complete@example.com");
        result.PhoneNumber.Should().Be("+19876543210");
        result.ExternalCode.Should().Be("EXT123");
        
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockOwnerRepository.Verify(x => x.Update(existingOwner), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithOnlyRequiredFields_ShouldUpdateSuccessfully()
    {
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdateOwnerDto
        {
            FirstName = "Min",
            LastName = "Owner",
            Email = "min@example.com",
            PhoneNumber = "+11111111111",
            AddressLine = "123 Main St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
            Country = "US"
        };
        var command = new UpdateOwnerCommand { Id = ownerId, Data = updateDto };

        var existingOwner = Owner.Create("Old Name", "old@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByEmailAsync(updateDto.Email, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("Min Owner");
        result.Email.Should().Be("min@example.com");
        result.PhoneNumber.Should().Be("+11111111111");
    }
}