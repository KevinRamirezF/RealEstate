using FluentAssertions;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Domain.Entities;

[TestFixture]
public class OwnerTests
{
    private const string ValidFullName = "John Smith";
    private const string ValidEmail = "john.smith@email.com";
    private const string ValidPhone = "+1-555-0123";
    private const string ValidExternalCode = "EXT001";

    [Test]
    public void Create_WithValidData_ShouldCreateOwner()
    {
        var owner = Owner.Create(ValidFullName, ValidEmail, ValidPhone, ValidExternalCode);

        owner.Should().NotBeNull();
        owner.Id.Should().NotBeEmpty();
        owner.FullName.Should().Be(ValidFullName);
        owner.Email.Should().Be(ValidEmail);
        owner.Phone.Should().Be(ValidPhone);
        owner.ExternalCode.Should().Be(ValidExternalCode);
        owner.IsActive.Should().BeTrue();
        owner.Country.Should().Be("US");
        owner.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        owner.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void Create_WithOnlyFullName_ShouldCreateOwner()
    {
        var owner = Owner.Create(ValidFullName);

        owner.FullName.Should().Be(ValidFullName);
        owner.Email.Should().BeNull();
        owner.Phone.Should().BeNull();
        owner.ExternalCode.Should().BeNull();
    }

    [Test]
    public void Create_WithEmptyFullName_ShouldThrowException()
    {
        var action = () => Owner.Create("");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Full name is required.*");
    }

    [Test]
    public void Create_WithWhitespaceFullName_ShouldThrowException()
    {
        var action = () => Owner.Create("   ");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Full name is required.*");
    }

    [Test]
    public void Create_WithNullFullName_ShouldThrowException()
    {
        var action = () => Owner.Create(null!);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Full name is required.*");
    }

    [Test]
    public void Create_WithTooLongFullName_ShouldThrowException()
    {
        var longName = new string('A', 181);

        var action = () => Owner.Create(longName);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 180 characters*");
    }

    [Test]
    public void Create_WithMaxValidFullName_ShouldCreateOwner()
    {
        var maxName = new string('A', 180);

        var owner = Owner.Create(maxName);

        owner.FullName.Should().Be(maxName);
    }

    [Test]
    public void Update_WithValidData_ShouldUpdateOwner()
    {
        var owner = Owner.Create(ValidFullName, ValidEmail, ValidPhone);
        var newName = "Jane Doe";
        var newEmail = "jane.doe@email.com";
        var newPhone = "+1-555-9876";
        var originalUpdateTime = owner.UpdatedAt;

        Thread.Sleep(10);
        owner.Update(newName, newEmail, newPhone);

        owner.FullName.Should().Be(newName);
        owner.Email.Should().Be(newEmail);
        owner.Phone.Should().Be(newPhone);
        owner.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Test]
    public void Update_WithNullOptionalFields_ShouldUpdateOwner()
    {
        var owner = Owner.Create(ValidFullName, ValidEmail, ValidPhone);
        var newName = "Jane Doe";

        owner.Update(newName, null, null);

        owner.FullName.Should().Be(newName);
        owner.Email.Should().BeNull();
        owner.Phone.Should().BeNull();
    }

    [Test]
    public void Update_WithEmptyFullName_ShouldThrowException()
    {
        var owner = Owner.Create(ValidFullName);

        var action = () => owner.Update("");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Full name is required.*");
    }

    [Test]
    public void Update_WithNullFullName_ShouldThrowException()
    {
        var owner = Owner.Create(ValidFullName);

        var action = () => owner.Update(null!);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Full name is required.*");
    }

    [Test]
    public void SoftDelete_ShouldSetDeletedAt()
    {
        var owner = Owner.Create(ValidFullName);

        owner.SoftDelete();

        owner.DeletedAt.Should().NotBeNull();
        owner.DeletedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void SoftDelete_ShouldUpdateUpdateTime()
    {
        var owner = Owner.Create(ValidFullName);
        var originalUpdateTime = owner.UpdatedAt;

        Thread.Sleep(10);
        owner.SoftDelete();

        owner.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Test]
    public void Constructor_ShouldSetDefaultValues()
    {
        var owner = Owner.Create(ValidFullName);

        owner.IsActive.Should().BeTrue();
        owner.Country.Should().Be("US");
        owner.BirthDate.Should().BeNull();
        owner.AddressLine.Should().BeNull();
        owner.City.Should().BeNull();
        owner.State.Should().BeNull();
        owner.PostalCode.Should().BeNull();
        owner.PhotoUrl.Should().BeNull();
        owner.DeletedAt.Should().BeNull();
    }

    [Test]
    public void RowVersion_ShouldBeInitialized()
    {
        var owner = Owner.Create(ValidFullName);

        owner.RowVersion.Should().NotBeNull();
        owner.RowVersion.Should().HaveCount(8);
    }
}