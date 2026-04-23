using System.Collections.Concurrent;
using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Repositories;

public class TemporalBlockRepository : ITemporalBlockRepository
{
    private readonly ConcurrentDictionary<string, TemporalBlock> _temporalBlocks = new(StringComparer.OrdinalIgnoreCase);

    public bool Add(TemporalBlock block)
    {
        return _temporalBlocks.TryAdd(block.CountryCode, block);
    }

    public bool Remove(string countryCode)
    {
        return _temporalBlocks.TryRemove(countryCode, out _);
    }

    public TemporalBlock? Get(string countryCode)
    {
        if (_temporalBlocks.TryGetValue(countryCode, out var block))
        {
            if (block.IsExpired)
            {
                _temporalBlocks.TryRemove(countryCode, out _);
                return null;
            }
            return block;
        }
        return null;
    }

    public IEnumerable<TemporalBlock> GetAll()
    {
        // Only return active blocks
        return _temporalBlocks.Values.Where(b => !b.IsExpired);
    }

    public bool Exists(string countryCode)
    {
        return Get(countryCode) != null;
    }

    public int RemoveExpired()
    {
        int removedCount = 0;
        foreach (var kvp in _temporalBlocks)
        {
            if (kvp.Value.IsExpired)
            {
                if (_temporalBlocks.TryRemove(kvp.Key, out _))
                {
                    removedCount++;
                }
            }
        }
        return removedCount;
    }
}
