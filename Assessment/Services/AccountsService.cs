using Assessment.Models;
using Assessment.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Assessment.Services;

public class AccountsService : IAccountsService
{
    private readonly IMongoCollection<Account> _accountsCollection;

    public AccountsService(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
    {
        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _accountsCollection = mongoDatabase.GetCollection<Account>(
            settings.Value.AccountsCollectionName);
    }
    
    public async Task CreateAsync(Account account) => await _accountsCollection.InsertOneAsync(account);
    
    public async Task UpdateAsync(Account account) => await _accountsCollection.ReplaceOneAsync(a => a.Id == account.Id, account);

    public async Task DeleteAsync(string id) => await _accountsCollection.DeleteOneAsync(x => x.Id == id);
    
    public async Task<Account?> GetByIdAsync(string id) => await _accountsCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
    
    public async Task<Account?> GetByUsernameAsync(string username) => 
        await _accountsCollection.Find(a => a.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();
    
    public async Task<Account?> GetByEmailAsync(string email) => 
        await _accountsCollection.Find(a => a.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();

    public async Task<long> CountSignUpConflicts(string username, string email) => 
        await _accountsCollection.CountDocumentsAsync(a => a.Username.ToLower() == username.ToLower() || a.Email.ToLower() == email.ToLower());
    
    public async Task<QueryResult<Account>> GetManyAsync(
        string? username,
        string? email,
        DateTime? createdAfter,
        DateTime? createdBefore,
        int limit = 100,
        int skip = 0
    )
    {
        // If any query parameters are provided, filter based on them
        IFindFluent<Account, Account> query = _accountsCollection.Find(
            a =>
                (username == null || a.Username.Contains(username)) &&
                (email == null || a.Email.Contains(email)) &&
                (createdBefore == null || a.Created < createdBefore) &&
                (createdAfter == null || a.Created > createdAfter)
        );
        
        long count = await query.CountDocumentsAsync();
        List<Account> accounts = await query.Limit(limit).Skip(skip).ToListAsync();

        return new QueryResult<Account>()
        {
            Limit = limit,
            Skip = skip,
            Count = count,
            Items = accounts
        };
    }
}
