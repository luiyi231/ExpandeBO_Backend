using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class CiudadRepository : ICiudadRepository
{
    private readonly IMongoCollection<Ciudad> _ciudades;

    public CiudadRepository(IMongoContext context)
    {
        _ciudades = context.Database.GetCollection<Ciudad>("ciudades");
    }

    public async Task<Ciudad?> GetByIdAsync(string id)
    {
        return await _ciudades.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Ciudad> CreateAsync(Ciudad ciudad)
    {
        await _ciudades.InsertOneAsync(ciudad);
        return ciudad;
    }

    public async Task<Ciudad> UpdateAsync(Ciudad ciudad)
    {
        await _ciudades.ReplaceOneAsync(c => c.Id == ciudad.Id, ciudad);
        return ciudad;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _ciudades.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Ciudad>> GetAllAsync()
    {
        return await _ciudades.Find(_ => true).ToListAsync();
    }

    public async Task<List<Ciudad>> GetActivasAsync()
    {
        return await _ciudades.Find(c => c.Activa).ToListAsync();
    }
}


