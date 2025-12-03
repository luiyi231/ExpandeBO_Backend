using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(string id);
    Task<Cliente?> GetByUsuarioIdAsync(string usuarioId);
    Task<List<Cliente>> GetByCiudadIdAsync(string ciudadId);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente> UpdateAsync(Cliente cliente);
    Task<bool> DeleteAsync(string id);
    Task<List<Cliente>> GetAllAsync();
}

