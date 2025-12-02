using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface ICiudadRepository
{
    Task<Ciudad?> GetByIdAsync(string id);
    Task<Ciudad> CreateAsync(Ciudad ciudad);
    Task<Ciudad> UpdateAsync(Ciudad ciudad);
    Task<bool> DeleteAsync(string id);
    Task<List<Ciudad>> GetAllAsync();
    Task<List<Ciudad>> GetActivasAsync();
}


