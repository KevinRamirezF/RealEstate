using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Queries.Owners;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Application.Queries;

public class GetOwnersQueryHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IOwnerRepository> _mockOwnerRepository;
    private OwnerMapper _mapper;
    private GetOwnersQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOwnerRepository = new Mock<IOwnerRepository>();
        _mapper = new OwnerMapper();

        _mockUnitOfWork.Setup(x => x.Owners).Returns(_mockOwnerRepository.Object);
        _handler = new GetOwnersQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Test]
    public async Task HandleAsync_ShouldReturnAllOwners()
    {
        var query = new GetOwnersQuery();

        var owners = new List<Owner>
        {
            Owner.Create("John Doe", "john.doe@example.com", "+1234567890", null),
            Owner.Create("John Smith", "john.smith@example.com", "+9876543210", null)
        };

        var expectedListDtos = new List<OwnerListDto>
        {
            new OwnerListDto
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new OwnerListDto
            {
                Id = Guid.NewGuid(),
                FullName = "John Smith", 
                Email = "john.smith@example.com",
                PhoneNumber = "+9876543210",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        _mockOwnerRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(owners);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(o => o.FullName.Contains("John")).Should().BeTrue();
    }

    [Test]
    public async Task HandleAsync_WithEmptyRepository_ShouldReturnEmptyResult()
    {
        var query = new GetOwnersQuery();

        _mockOwnerRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Owner>());

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockOwnerRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}