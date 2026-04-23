using System.Collections.Concurrent;
using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Repositories;

public class BlockAttemptLogRepository : IBlockAttemptLogRepository
{
    // ConcurrentBag is ideal for a fast append-only collection
    private readonly ConcurrentBag<BlockAttemptLog> _logs = new();

    public void Add(BlockAttemptLog log)
    {
        _logs.Add(log);
    }

    public IEnumerable<BlockAttemptLog> GetAll()
    {
        return _logs;
    }
}
