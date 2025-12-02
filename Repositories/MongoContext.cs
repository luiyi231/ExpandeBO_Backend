using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class MongoContext : IMongoContext
{
    public IMongoDatabase Database { get; }

    public MongoContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(databaseName);
    }
}


