using BlockedCountriesApi.Helpers;

namespace BlockedCountriesApi.Tests;

public class CountryCodeValidatorTests
{
    [Theory]
    [InlineData("US")]
    [InlineData("GB")]
    [InlineData("EG")]
    [InlineData("DE")]
    [InlineData("JP")]
    [InlineData("AU")]
    [InlineData("SA")]
    [InlineData("AE")]
    public void IsValid_KnownCodes_ReturnsTrue(string code)
    {
        Assert.True(CountryCodeValidator.IsValid(code));
    }

    [Theory]
    [InlineData("XX")]
    [InlineData("ZZ")]
    [InlineData("AA")]
    [InlineData("QQ")]
    public void IsValid_UnknownCodes_ReturnsFalse(string code)
    {
        Assert.False(CountryCodeValidator.IsValid(code));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void IsValid_EmptyOrNull_ReturnsFalse(string? code)
    {
        Assert.False(CountryCodeValidator.IsValid(code!));
    }

    [Fact]
    public void IsValid_LowercaseCode_ReturnsTrueViaCaseInsensitivity()
    {
        // The HashSet is case-insensitive
        Assert.True(CountryCodeValidator.IsValid("us"));
        Assert.True(CountryCodeValidator.IsValid("gb"));
    }
}
