using System.Collections.Concurrent;
using BlockedCountriesApi.Models;

namespace BlockedCountriesApi.Repositories;

public interface IBlockAttemptLogRepository
{
    void Add(BlockAttemptLog log);
    IEnumerable<BlockAttemptLog> GetAll();
}
