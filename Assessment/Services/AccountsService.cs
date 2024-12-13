using Assessment.Models;
using Assessment.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Assessment.Services;

public class AccountsService
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
    
    public async Task<Account?> GetByUsernameAsync(string username) => await _accountsCollection.Find(a => a.Username == username).FirstOrDefaultAsync();
    
    public async Task<Account?> GetByEmailAsync(string email) => await _accountsCollection.Find(a => a.Email == email).FirstOrDefaultAsync();
}
