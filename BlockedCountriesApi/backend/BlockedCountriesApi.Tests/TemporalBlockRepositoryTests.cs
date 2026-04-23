using BlockedCountriesApi.Models;
using BlockedCountriesApi.Repositories;

namespace BlockedCountriesApi.Tests;

public class TemporalBlockRepositoryTests
{
    private TemporalBlock MakeBlock(string code, int minutesFromNow) => new()
    {
        CountryCode = code,
        CountryName = "Test Country",
        BlockedAt   = DateTime.UtcNow,
        DurationMinutes = Math.Abs(minutesFromNow),
        ExpiresAt   = DateTime.UtcNow.AddMinutes(minutesFromNow)
    };

    [Fact]
    public void Add_NewBlock_ReturnsTrue()
    {
        var repo = new TemporalBlockRepository();
        Assert.True(repo.Add(MakeBlock("US", 60)));
    }

    [Fact]
    public void Add_DuplicateBlock_ReturnsFalse()
    {
        var repo = new TemporalBlockRepository();
        repo.Add(MakeBlock("US", 60));
        Assert.False(repo.Add(MakeBlock("US", 30)));
    }

    [Fact]
    public void Get_ActiveBlock_ReturnsBlock()
    {
        var repo = new TemporalBlockRepository();
        repo.Add(MakeBlock("GB", 60));
        Assert.NotNull(repo.Get("GB"));
    }

    [Fact]
    public void Get_ExpiredBlock_ReturnsNull()
    {
        var repo = new TemporalBlockRepository();
        repo.Add(MakeBlock("DE", -1)); // Expired 1 minute ago
        Assert.Null(repo.Get("DE"));
    }

    [Fact]
    public void Exists_ExpiredBlock_ReturnsFalse()
    {
        var repo = new TemporalBlockRepository();
        repo.Add(MakeBlock("FR", -5));
        Assert.False(repo.Exists("FR"));
    }

    [Fact]
    public void RemoveExpired_RemovesOnlyExpired()
    {
        var repo = new TemporalBlockRepository();
        repo.Add(MakeBlock("US", 60));  // Active
        repo.Add(MakeBlock("GB", -1)); // Expired
        repo.Add(MakeBlock("DE", -5)); // Expired

        int removed = repo.RemoveExpired();

        Assert.Equal(2, removed);
        Assert.True(repo.Exists("US"));
        Assert.False(repo.Exists("GB"));
        Assert.False(repo.Exists("DE"));
    }

    [Fact]
    public void GetAll_ReturnsOnlyActiveBlocks()
    {
        var repo = new TemporalBlockRepository();
        repo.Add(MakeBlock("US", 60));  // Active
        repo.Add(MakeBlock("GB", -1)); // Expired

        var all = repo.GetAll().ToList();

        Assert.Single(all);
        Assert.Equal("US", all[0].CountryCode);
    }

    [Fact]
    public void Remove_ExistingBlock_ReturnsTrue()
    {
        var repo = new TemporalBlockRepository();
        repo.Add(MakeBlock("JP", 60));
        Assert.True(repo.Remove("JP"));
        Assert.False(repo.Exists("JP"));
    }
}
