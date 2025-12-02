using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(string id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario> CreateAsync(Usuario usuario);
    Task<Usuario> UpdateAsync(Usuario usuario);
    Task<bool> DeleteAsync(string id);
    Task<List<Usuario>> GetAllAsync();
}


