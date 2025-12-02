using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public interface IMongoContext
{
    IMongoDatabase Database { get; }
}


