using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly IMongoCollection<Categoria> _categorias;

    public CategoriaRepository(IMongoContext context)
    {
        _categorias = context.Database.GetCollection<Categoria>("categorias");
    }

    public async Task<Categoria?> GetByIdAsync(string id)
    {
        return await _categorias.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Categoria> CreateAsync(Categoria categoria)
    {
        await _categorias.InsertOneAsync(categoria);
        return categoria;
    }

    public async Task<Categoria> UpdateAsync(Categoria categoria)
    {
        await _categorias.ReplaceOneAsync(c => c.Id == categoria.Id, categoria);
        return categoria;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _categorias.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Categoria>> GetAllAsync()
    {
        return await _categorias.Find(_ => true).ToListAsync();
    }

    public async Task<List<Categoria>> GetActivasAsync()
    {
        return await _categorias.Find(c => c.Activa).ToListAsync();
    }
}


