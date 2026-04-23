using BlockedCountriesApi.Models;
using BlockedCountriesApi.Repositories;
using BlockedCountriesApi.Services;
using Moq;

namespace BlockedCountriesApi.Tests;

public class CountryServiceTests
{
    private readonly Mock<IBlockedCountryRepository> _mockBlockedRepo;
    private readonly Mock<ITemporalBlockRepository> _mockTemporalRepo;
    private readonly CountryService _service;

    public CountryServiceTests()
    {
        _mockBlockedRepo = new Mock<IBlockedCountryRepository>();
        _mockTemporalRepo = new Mock<ITemporalBlockRepository>();
        _service = new CountryService(_mockBlockedRepo.Object, _mockTemporalRepo.Object);
    }

    // ────────────── BlockCountry ──────────────

    [Fact]
    public void BlockCountry_ValidNewCode_ReturnsTrue()
    {
        _mockBlockedRepo.Setup(r => r.Exists("US")).Returns(false);
        _mockBlockedRepo.Setup(r => r.Add(It.IsAny<BlockedCountry>())).Returns(true);

        var result = _service.BlockCountry("US");

        Assert.True(result);
        _mockBlockedRepo.Verify(r => r.Add(It.Is<BlockedCountry>(c => c.CountryCode == "US")), Times.Once);
    }

    [Fact]
    public void BlockCountry_AlreadyBlocked_ReturnsFalse()
    {
        _mockBlockedRepo.Setup(r => r.Exists("US")).Returns(true);

        var result = _service.BlockCountry("US");

        Assert.False(result);
        _mockBlockedRepo.Verify(r => r.Add(It.IsAny<BlockedCountry>()), Times.Never);
    }

    [Fact]
    public void BlockCountry_InvalidCode_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.BlockCountry("XX"));
    }

    [Fact]
    public void BlockCountry_AcceptsLowercase_ConvertsToUpper()
    {
        _mockBlockedRepo.Setup(r => r.Exists("GB")).Returns(false);
        _mockBlockedRepo.Setup(r => r.Add(It.IsAny<BlockedCountry>())).Returns(true);

        var result = _service.BlockCountry("gb");

        Assert.True(result);
        _mockBlockedRepo.Verify(r => r.Add(It.Is<BlockedCountry>(c => c.CountryCode == "GB")), Times.Once);
    }

    // ────────────── UnblockCountry ──────────────

    [Fact]
    public void UnblockCountry_ExistingPermanentBlock_ReturnsTrue()
    {
        _mockBlockedRepo.Setup(r => r.Remove("US")).Returns(true);
        _mockTemporalRepo.Setup(r => r.Remove("US")).Returns(false);

        var result = _service.UnblockCountry("US");

        Assert.True(result);
    }

    [Fact]
    public void UnblockCountry_ExistingTemporalBlock_ReturnsTrue()
    {
        _mockBlockedRepo.Setup(r => r.Remove("DE")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Remove("DE")).Returns(true);

        var result = _service.UnblockCountry("DE");

        Assert.True(result);
    }

    [Fact]
    public void UnblockCountry_NotBlocked_ReturnsFalse()
    {
        _mockBlockedRepo.Setup(r => r.Remove("FR")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Remove("FR")).Returns(false);

        var result = _service.UnblockCountry("FR");

        Assert.False(result);
    }

    // ────────────── IsCountryBlocked ──────────────

    [Fact]
    public void IsCountryBlocked_PermanentlyBlocked_ReturnsTrue()
    {
        _mockBlockedRepo.Setup(r => r.Exists("EG")).Returns(true);

        Assert.True(_service.IsCountryBlocked("EG"));
    }

    [Fact]
    public void IsCountryBlocked_TemporallyBlocked_ReturnsTrue()
    {
        _mockBlockedRepo.Setup(r => r.Exists("CN")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Exists("CN")).Returns(true);

        Assert.True(_service.IsCountryBlocked("CN"));
    }

    [Fact]
    public void IsCountryBlocked_NotBlocked_ReturnsFalse()
    {
        _mockBlockedRepo.Setup(r => r.Exists("JP")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Exists("JP")).Returns(false);

        Assert.False(_service.IsCountryBlocked("JP"));
    }

    [Fact]
    public void IsCountryBlocked_EmptyCode_ReturnsFalse()
    {
        Assert.False(_service.IsCountryBlocked(""));
    }

    // ────────────── TemporalBlockCountry ──────────────

    [Fact]
    public void TemporalBlockCountry_ValidCode_ReturnsTrue()
    {
        _mockBlockedRepo.Setup(r => r.Exists("BR")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Exists("BR")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Add(It.IsAny<TemporalBlock>())).Returns(true);

        var result = _service.TemporalBlockCountry("BR", 60);

        Assert.True(result);
        _mockTemporalRepo.Verify(r => r.Add(It.Is<TemporalBlock>(t =>
            t.CountryCode == "BR" && t.DurationMinutes == 60)), Times.Once);
    }

    [Fact]
    public void TemporalBlockCountry_InvalidCode_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.TemporalBlockCountry("ZZ", 60));
    }

    [Fact]
    public void TemporalBlockCountry_AlreadyPermanentlyBlocked_ReturnsFalse()
    {
        _mockBlockedRepo.Setup(r => r.Exists("IN")).Returns(true);

        var result = _service.TemporalBlockCountry("IN", 30);

        Assert.False(result);
        _mockTemporalRepo.Verify(r => r.Add(It.IsAny<TemporalBlock>()), Times.Never);
    }

    [Fact]
    public void TemporalBlockCountry_AlreadyTemporarilyBlocked_ReturnsFalse()
    {
        _mockBlockedRepo.Setup(r => r.Exists("SA")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Exists("SA")).Returns(true);

        var result = _service.TemporalBlockCountry("SA", 120);

        Assert.False(result);
    }

    [Fact]
    public void TemporalBlockCountry_ExpiresAtIsCorrect()
    {
        var before = DateTime.UtcNow;
        _mockBlockedRepo.Setup(r => r.Exists("AU")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Exists("AU")).Returns(false);
        _mockTemporalRepo.Setup(r => r.Add(It.IsAny<TemporalBlock>())).Returns(true);

        _service.TemporalBlockCountry("AU", 120);

        _mockTemporalRepo.Verify(r => r.Add(It.Is<TemporalBlock>(t =>
            t.ExpiresAt >= before.AddMinutes(120) &&
            t.ExpiresAt <= DateTime.UtcNow.AddMinutes(120))), Times.Once);
    }

    // ────────────── GetBlockedCountries (Search) ──────────────

    [Fact]
    public void GetBlockedCountries_SearchByCode_FiltersCorrectly()
    {
        _mockBlockedRepo.Setup(r => r.GetAll()).Returns(new List<BlockedCountry>
        {
            new() { CountryCode = "US", CountryName = "United States", BlockedAt = DateTime.UtcNow },
            new() { CountryCode = "GB", CountryName = "United Kingdom", BlockedAt = DateTime.UtcNow }
        });
        _mockTemporalRepo.Setup(r => r.GetAll()).Returns(Enumerable.Empty<TemporalBlock>());

        var result = _service.GetBlockedCountries(1, 10, "US");

        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public void GetBlockedCountries_NoSearch_ReturnsAll()
    {
        _mockBlockedRepo.Setup(r => r.GetAll()).Returns(new List<BlockedCountry>
        {
            new() { CountryCode = "US", CountryName = "United States", BlockedAt = DateTime.UtcNow },
            new() { CountryCode = "EG", CountryName = "Egypt",         BlockedAt = DateTime.UtcNow }
        });
        _mockTemporalRepo.Setup(r => r.GetAll()).Returns(Enumerable.Empty<TemporalBlock>());

        var result = _service.GetBlockedCountries(1, 10, null);

        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public void GetBlockedCountries_Pagination_ReturnsCorrectPage()
    {
        var countries = Enumerable.Range(1, 25).Select(i => new BlockedCountry
        {
            CountryCode = "C" + i.ToString("D1"),
            CountryName = "Country " + i,
            BlockedAt   = DateTime.UtcNow
        }).ToList();

        _mockBlockedRepo.Setup(r => r.GetAll()).Returns(countries);
        _mockTemporalRepo.Setup(r => r.GetAll()).Returns(Enumerable.Empty<TemporalBlock>());

        var result = _service.GetBlockedCountries(2, 10, null);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(10, result.Items.Count());
        Assert.Equal(3, result.TotalPages);
    }
}
