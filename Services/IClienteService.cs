using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Services;

public interface IClienteService
{
    Task<Cliente> CreateClienteAsync(string usuarioId, string ciudadId, string? direccion = null);
    Task<Cliente?> GetClienteByUsuarioIdAsync(string usuarioId);
    Task<Cliente?> GetClienteByIdAsync(string id);
    Task<Cliente> UpdateClienteAsync(string clienteId, string ciudadId, string? direccion = null);
    Task<List<Cliente>> GetClientesByCiudadAsync(string ciudadId);
    Task<bool> DeleteClienteAsync(string clienteId);
    Task<List<Cliente>> GetAllClientesAsync();
}

