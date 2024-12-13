using Assessment.Models;
using Assessment.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Assessment.Services;

public class ProfilesService
{
    private readonly IMongoCollection<Profile> _profilesCollection;

    public ProfilesService(IMongoClient mongoClient, IOptions<DatabaseSettings> settings)
    {
        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _profilesCollection = mongoDatabase.GetCollection<Profile>(
            settings.Value.ProfilesCollectionName);
    }
    
    public async Task<Profile?> GetByIdAsync(string id) => await _profilesCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
    
    public async Task<Profile?> GetByEmailAsync(string email) => await _profilesCollection.Find(p => p.Email == email).FirstOrDefaultAsync();
    
    public async Task CreateAsync(Profile profile) => await _profilesCollection.InsertOneAsync(profile);
    
    public async Task UpdateAsync(Profile profile) => await _profilesCollection.ReplaceOneAsync(p => p.Id == profile.Id, profile);

    public async Task DeleteAsync(string id) => await _profilesCollection.DeleteOneAsync(x => x.Id == id);
}