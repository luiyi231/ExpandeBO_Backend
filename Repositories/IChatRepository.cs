using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IChatRepository
{
    Task<Chat?> GetByIdAsync(string id);
    Task<Chat?> GetByClienteAndPerfilAsync(string clienteId, string perfilComercialId);
    Task<Chat?> GetSoporteByClienteAsync(string clienteId);
    Task<List<Chat>> GetByClienteIdAsync(string clienteId);
    Task<List<Chat>> GetByPerfilComercialIdAsync(string perfilComercialId);
    Task<List<Chat>> GetSoporteAsync();
    Task<Chat> CreateAsync(Chat chat);
    Task<Chat> UpdateAsync(Chat chat);
    Task<bool> DeleteAsync(string id);
}


