using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface ISubcategoriaRepository
{
    Task<Subcategoria?> GetByIdAsync(string id);
    Task<List<Subcategoria>> GetByCategoriaIdAsync(string categoriaId);
    Task<Subcategoria> CreateAsync(Subcategoria subcategoria);
    Task<Subcategoria> UpdateAsync(Subcategoria subcategoria);
    Task<bool> DeleteAsync(string id);
    Task<List<Subcategoria>> GetAllAsync();
    Task<List<Subcategoria>> GetActivasAsync();
}


