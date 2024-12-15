using Assessment.Models;

namespace Assessment.Services;

public interface IProfilesService
{
    Task<Profile?> GetByIdAsync(string id);
    Task<long> CountByIdAsync(string id);
    Task CreateAsync(Profile profile);
    Task UpdateAsync(Profile profile);
    Task DeleteAsync(string id);
    Task<QueryResult<Profile>> GetManyAsync(
        string? name,
        string? email,
        string? phoneNumber,
        string? city,
        string? state,
        string? postalCode,
        DateTime? createdAfter,
        DateTime? createdBefore,
        int limit = 100,
        int skip = 0
    );

    Task SyncWithAccount(Account account);
}