using Assessment.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Assessment.Services;

public class AUsersService
{
    private readonly IMongoCollection<AUser> _usersCollection;

    public AUsersService(IMongoClient mongoClient, IOptions<AssessmentDatabaseSettings> settings)
    {
        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<AUser>(
            settings.Value.UsersCollectionName);
    }
    
    public async Task<AUser?> GetAsync(string id) => await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    
    public async Task<AUser?> GetByUsernameAsync(string username) => await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
    
    public async Task CreateAsync(AUser aUser) => await _usersCollection.InsertOneAsync(aUser);
    
    public async Task UpdateAsync(AUser aUser) => await _usersCollection.ReplaceOneAsync(u => u.Id == aUser.Id, aUser);

    public async Task DeleteAsync(string id) => await _usersCollection.DeleteOneAsync(x => x.Id == id);
}
