using Assessment.Models;
using Assessment.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Assessment.Services;

public class AccountsService
{
    private readonly IMongoCollection<Account> _usersCollection;

    public AccountsService(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
    {
        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<Account>(
            settings.Value.AccountsCollectionName);
    }
    
    public async Task<Account?> GetByIdAsync(string id) => await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    
    public async Task<Account?> GetByUsernameAsync(string username) => await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
    
    public async Task CreateAsync(Account account) => await _usersCollection.InsertOneAsync(account);
    
    public async Task UpdateAsync(Account account) => await _usersCollection.ReplaceOneAsync(u => u.Id == account.Id, account);

    public async Task DeleteAsync(string id) => await _usersCollection.DeleteOneAsync(x => x.Id == id);
}
