using Assessment.Models;

namespace Assessment.Services;

public interface IAccountsService
{
    Task CreateAsync(Account account);
    
    Task UpdateAsync(Account account);
    
    Task DeleteAsync(string id);
    
    Task<Account?> GetByIdAsync(string id);
    
    Task<Account?> GetByUsernameAsync(string username);
    
    Task<Account?> GetByEmailAsync(string email);
    
    Task<long> CountSignUpConflicts(string username, string email);
    

    Task<QueryResult<Account>> GetManyAsync(
        string? username,
        string? email,
        DateTime? createdAfter,
        DateTime? createdBefore,
        int limit = 100,
        int skip = 0
    );
}