using FluentAssertions;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.UnitTests.Domain.Entities;

[TestFixture]
public class PropertyImageTests
{
    private Guid _propertyId;
    private const string ValidUrl = "https://example.com/image.jpg";
    private const string ValidAltText = "Beautiful property image";

    [SetUp]
    public void Setup()
    {
        _propertyId = Guid.NewGuid();
    }

    [Test]
    public void Create_WithValidData_ShouldCreatePropertyImage()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl, StorageProvider.S3, ValidAltText, true, 1);

        image.Should().NotBeNull();
        image.Id.Should().NotBeEmpty();
        image.PropertyId.Should().Be(_propertyId);
        image.Url.Should().Be(ValidUrl);
        image.StorageProvider.Should().Be(StorageProvider.S3);
        image.AltText.Should().Be(ValidAltText);
        image.IsPrimary.Should().BeTrue();
        image.SortOrder.Should().Be(1);
        image.Enabled.Should().BeTrue();
        image.RowVersion.Should().Be(1);
        image.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        image.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void Create_WithMinimalData_ShouldCreatePropertyImageWithDefaults()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl);

        image.PropertyId.Should().Be(_propertyId);
        image.Url.Should().Be(ValidUrl);
        image.StorageProvider.Should().Be(StorageProvider.S3);
        image.AltText.Should().BeNull();
        image.IsPrimary.Should().BeFalse();
        image.SortOrder.Should().Be(0);
        image.Enabled.Should().BeTrue();
        image.DeletedAt.Should().BeNull();
    }

    [Test]
    public void Create_WithEmptyUrl_ShouldThrowException()
    {
        var action = () => PropertyImage.Create(_propertyId, "");

        action.Should().Throw<ArgumentException>()
            .WithMessage("URL is required.*");
    }

    [Test]
    public void Create_WithWhitespaceUrl_ShouldThrowException()
    {
        var action = () => PropertyImage.Create(_propertyId, "   ");

        action.Should().Throw<ArgumentException>()
            .WithMessage("URL is required.*");
    }

    [Test]
    public void Create_WithNullUrl_ShouldThrowException()
    {
        var action = () => PropertyImage.Create(_propertyId, null!);

        action.Should().Throw<ArgumentException>()
            .WithMessage("URL is required.*");
    }

    [Test]
    public void Create_WithTooLongUrl_ShouldThrowException()
    {
        var longUrl = "https://example.com/" + new string('a', 1000);

        var action = () => PropertyImage.Create(_propertyId, longUrl);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 1000 characters*");
    }

    [Test]
    public void Create_WithMaxValidUrl_ShouldCreatePropertyImage()
    {
        var maxUrl = "https://example.com/" + new string('a', 980);

        var image = PropertyImage.Create(_propertyId, maxUrl);

        image.Url.Should().Be(maxUrl);
    }

    [Test]
    public void SetAsPrimary_WithTrue_ShouldSetPrimaryAndUpdateVersion()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl);
        var originalVersion = image.RowVersion;
        var originalUpdateTime = image.UpdatedAt;

        Thread.Sleep(10);
        image.SetAsPrimary(true);

        image.IsPrimary.Should().BeTrue();
        image.RowVersion.Should().Be(originalVersion + 1);
        image.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Test]
    public void SetAsPrimary_WithFalse_ShouldUnsetPrimaryAndUpdateVersion()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl, isPrimary: true);
        var originalVersion = image.RowVersion;

        image.SetAsPrimary(false);

        image.IsPrimary.Should().BeFalse();
        image.RowVersion.Should().Be(originalVersion + 1);
    }

    [Test]
    public void UpdateSortOrder_ShouldUpdateOrderAndVersion()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl);
        var originalVersion = image.RowVersion;
        var originalUpdateTime = image.UpdatedAt;
        short newOrder = 5;

        Thread.Sleep(10);
        image.UpdateSortOrder(newOrder);

        image.SortOrder.Should().Be(newOrder);
        image.RowVersion.Should().Be(originalVersion + 1);
        image.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Test]
    public void SetEnabled_WithFalse_ShouldDisableImageAndUpdateVersion()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl);
        var originalVersion = image.RowVersion;
        var originalUpdateTime = image.UpdatedAt;

        Thread.Sleep(10);
        image.SetEnabled(false);

        image.Enabled.Should().BeFalse();
        image.RowVersion.Should().Be(originalVersion + 1);
        image.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Test]
    public void SetEnabled_WithTrue_ShouldEnableImageAndUpdateVersion()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl);
        image.SetEnabled(false);
        var originalVersion = image.RowVersion;

        image.SetEnabled(true);

        image.Enabled.Should().BeTrue();
        image.RowVersion.Should().Be(originalVersion + 1);
    }

    [Test]
    public void SoftDelete_ShouldSetDeletedAtAndUpdateVersion()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl);
        var originalVersion = image.RowVersion;
        var originalUpdateTime = image.UpdatedAt;

        Thread.Sleep(10);
        image.SoftDelete();

        image.DeletedAt.Should().NotBeNull();
        image.DeletedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        image.RowVersion.Should().Be(originalVersion + 1);
        image.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Test]
    public void Create_WithDifferentStorageProviders_ShouldCreateCorrectly()
    {
        var s3Image = PropertyImage.Create(_propertyId, ValidUrl, StorageProvider.S3);
        var azureImage = PropertyImage.Create(_propertyId, ValidUrl, StorageProvider.AZURE);
        var localImage = PropertyImage.Create(_propertyId, ValidUrl, StorageProvider.LOCAL);

        s3Image.StorageProvider.Should().Be(StorageProvider.S3);
        azureImage.StorageProvider.Should().Be(StorageProvider.AZURE);
        localImage.StorageProvider.Should().Be(StorageProvider.LOCAL);
    }

    [Test]
    public void MultipleOperations_ShouldIncrementVersionCorrectly()
    {
        var image = PropertyImage.Create(_propertyId, ValidUrl);
        var initialVersion = image.RowVersion;

        image.SetAsPrimary(true);
        image.UpdateSortOrder(3);
        image.SetEnabled(false);

        image.RowVersion.Should().Be(initialVersion + 3);
    }
}