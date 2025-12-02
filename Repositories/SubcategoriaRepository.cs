using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class SubcategoriaRepository : ISubcategoriaRepository
{
    private readonly IMongoCollection<Subcategoria> _subcategorias;

    public SubcategoriaRepository(IMongoContext context)
    {
        _subcategorias = context.Database.GetCollection<Subcategoria>("subcategorias");
    }

    public async Task<Subcategoria?> GetByIdAsync(string id)
    {
        return await _subcategorias.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Subcategoria>> GetByCategoriaIdAsync(string categoriaId)
    {
        return await _subcategorias.Find(s => s.CategoriaId == categoriaId).ToListAsync();
    }

    public async Task<Subcategoria> CreateAsync(Subcategoria subcategoria)
    {
        await _subcategorias.InsertOneAsync(subcategoria);
        return subcategoria;
    }

    public async Task<Subcategoria> UpdateAsync(Subcategoria subcategoria)
    {
        await _subcategorias.ReplaceOneAsync(s => s.Id == subcategoria.Id, subcategoria);
        return subcategoria;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _subcategorias.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Subcategoria>> GetAllAsync()
    {
        return await _subcategorias.Find(_ => true).ToListAsync();
    }

    public async Task<List<Subcategoria>> GetActivasAsync()
    {
        return await _subcategorias.Find(s => s.Activa).ToListAsync();
    }
}


