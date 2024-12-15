using Assessment.Models;
using Assessment.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Assessment.Services;

public class ProfilesService : IProfilesService
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
    
    public async Task<long> CountByIdAsync(string id) => await _profilesCollection.CountDocumentsAsync(p => p.Id == id);
    
    public async Task CreateAsync(Profile profile) => await _profilesCollection.InsertOneAsync(profile);
    
    public async Task UpdateAsync(Profile profile) => await _profilesCollection.ReplaceOneAsync(p => p.Id == profile.Id, profile);

    public async Task DeleteAsync(string id) => await _profilesCollection.DeleteOneAsync(x => x.Id == id);
    
    public async Task<QueryResult<Profile>> GetManyAsync(
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
    )
    {
        // If any query parameters are provided, filter based on them
        IFindFluent<Profile, Profile> query= _profilesCollection.Find(
            p =>
                (string.IsNullOrEmpty(name) || (
                    p.Name.First.Contains(name) || 
                    (p.Name.Middle != null && p.Name.Middle.Contains(name)) || 
                    p.Name.Last.Contains(name)
                    )) &&
                (string.IsNullOrEmpty(email) || p.Email.Contains(email)) &&
                (string.IsNullOrEmpty(phoneNumber) || p.PhoneNumber.Contains(phoneNumber)) &&
                (string.IsNullOrEmpty(city) || p.Address.City.Contains(city)) &&
                (string.IsNullOrEmpty(state) || p.Address.State.Contains(state)) &&
                (string.IsNullOrEmpty(postalCode) || p.Address.PostalCode.Contains(postalCode)) &&
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