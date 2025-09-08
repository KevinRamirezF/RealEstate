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

public class PatchOwnerCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IOwnerRepository> _mockOwnerRepository;
    private OwnerMapper _mapper;
    private PatchOwnerDtoValidator _validator;
    private PatchOwnerCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOwnerRepository = new Mock<IOwnerRepository>();
        _mapper = new OwnerMapper();
        _validator = new PatchOwnerDtoValidator();

        _mockUnitOfWork.Setup(x => x.Owners).Returns(_mockOwnerRepository.Object);

        _handler = new PatchOwnerCommandHandler(_mockUnitOfWork.Object, _mapper, _validator);
    }

    [Test]
    public async Task HandleAsync_WithValidData_ShouldUpdateOwnerSuccessfully()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
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
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Old Name", "old@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByEmailAsync(patchDto.Email, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockOwnerRepository.Setup(x => x.ExistsByExternalCodeAsync(patchDto.ExternalCode, ownerId, It.IsAny<CancellationToken>()))
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
        var patchDto = new PatchOwnerDto
        {
            FirstName = "Updated",
            LastName = "Owner"
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Owner)null);

        var result = await _handler.HandleAsync(command);

        result.Should().BeNull();
    }

    [Test]
    public async Task HandleAsync_WithDuplicateEmail_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
        {
            Email = "existing@example.com"
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Test Owner", "old@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByEmailAsync(patchDto.Email, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("An owner with this email already exists.");
    }

    [Test]
    public async Task HandleAsync_WithDuplicateExternalCode_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
        {
            ExternalCode = "EXISTING_CODE"
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByExternalCodeAsync(patchDto.ExternalCode, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("An owner with this external code already exists.");
    }

    [Test]
    public async Task HandleAsync_WithOnlyPhoneNumber_ShouldUpdatePhoneOnly()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
        {
            PhoneNumber = "+19876543210"
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.PhoneNumber.Should().Be("+19876543210");
        result.FullName.Should().Be("Test Owner"); // Should remain unchanged
        result.Email.Should().Be("test@example.com"); // Should remain unchanged
    }

    [Test]
    public async Task HandleAsync_WithOnlyAddressFields_ShouldUpdateAddressOnly()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
        {
            AddressLine = "New Address",
            City = "New City",
            State = "CA",
            PostalCode = "90210",
            Country = "US"
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("Test Owner"); // Should remain unchanged
        result.Email.Should().Be("test@example.com"); // Should remain unchanged
        result.PhoneNumber.Should().Be("+1234567890"); // Should remain unchanged
    }

    [Test]
    public async Task HandleAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
        {
            Email = "invalid-email", // Invalid email format
            PhoneNumber = "invalid123" // Invalid phone format
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HandleAsync_WithPartialNameUpdate_ShouldCombineNames()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
        {
            FirstName = "UpdatedFirst"
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Original LastName", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("UpdatedFirst LastName");
    }

    [Test]
    public async Task HandleAsync_WithDateOfBirth_ShouldUpdateBirthDate()
    {
        var ownerId = Guid.NewGuid();
        var birthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));
        var patchDto = new PatchOwnerDto
        {
            DateOfBirth = birthDate
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task HandleAsync_WithExternalCode_ShouldUpdateExternalCode()
    {
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchOwnerDto
        {
            ExternalCode = "NEW_EXT_CODE"
        };
        var command = new PatchOwnerCommand { Id = ownerId, Data = patchDto };

        var existingOwner = Owner.Create("Test Owner", "test@example.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOwner);
        _mockOwnerRepository.Setup(x => x.ExistsByExternalCodeAsync(patchDto.ExternalCode, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.ExternalCode.Should().Be("NEW_EXT_CODE");
    }
}