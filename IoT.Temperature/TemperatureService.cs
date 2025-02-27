using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IoT.Temperature;


//TODO : Implement Polly(circuitBreakerPolicy with fallback policy).Have second variant of storing data if mongo isn't available
public class TemperatureService
{
    private readonly IMongoCollection<ServerThermometersReport> _booksCollection;

    //TODO: Implement error handling(or use EF Core)
    public TemperatureService(
        IOptions<ModuleDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);

        _booksCollection = mongoDatabase.GetCollection<ServerThermometersReport>(
            bookStoreDatabaseSettings.Value.TemperatureCollectionName);
    }
    
    public async Task<List<ServerThermometersReport>> GetAsync(TimeSpan  timeOffset) =>
        await _booksCollection.Find(temp => temp.DateTime >= DateTime.Now -timeOffset).ToListAsync();
    
    public async Task<List<ServerThermometersReport>> GetLastRecordsAsync(int  count) =>
         await _booksCollection.Find(FilterDefinition<ServerThermometersReport>.Empty)
            .Sort(Builders<ServerThermometersReport>.Sort.Descending(r => r.DateTime))
            .Limit(count)
            .ToListAsync();

    public async Task<ServerThermometersReport?> GetAsync(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(ServerThermometersReport newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, ServerThermometersReport updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);
}