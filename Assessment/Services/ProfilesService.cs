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
    
    public async Task<QueryResult<Profile>> GetManyAsync(
        string? name,
        string? email,
        string? phoneNumber,
        // TODO: Add searching by city, state, etc.
        DateTime? createdAfter,
        DateTime? createdBefore,
        int limit = 100,
        int skip = 0
    )
    {
        // If any query parameters are provided, filter based on them
        IFindFluent<Profile, Profile> query= _profilesCollection.Find(
            p =>
                (name == null || p.Name.First.Contains(name) || p.Name.Middle.Contains(name) || p.Name.Last.Contains(name)) &&
                (email == null || p.Email.Contains(email)) &&
                (phoneNumber == null || p.PhoneNumber.Contains(phoneNumber)) &&
                (createdBefore == null || p.Created < createdBefore) &&
                (createdAfter == null || p.Created > createdAfter)
        );
        
        long count = await query.CountDocumentsAsync();
        List<Profile> profiles = await query.Limit(limit).Skip(skip).ToListAsync();

        return new QueryResult<Profile>()
        {
            Limit = limit,
            Skip = skip,
            Count = count,
            Items = profiles
        };
    }
}