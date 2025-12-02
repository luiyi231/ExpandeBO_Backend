using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(string id);
    Task<Categoria> CreateAsync(Categoria categoria);
    Task<Categoria> UpdateAsync(Categoria categoria);
    Task<bool> DeleteAsync(string id);
    Task<List<Categoria>> GetAllAsync();
    Task<List<Categoria>> GetActivasAsync();
}


