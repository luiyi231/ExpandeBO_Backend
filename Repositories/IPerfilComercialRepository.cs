using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IPerfilComercialRepository
{
    Task<PerfilComercial?> GetByIdAsync(string id);
    Task<List<PerfilComercial>> GetByEmpresaIdAsync(string empresaId);
    Task<PerfilComercial> CreateAsync(PerfilComercial perfil);
    Task<PerfilComercial> UpdateAsync(PerfilComercial perfil);
    Task<bool> DeleteAsync(string id);
    Task<List<PerfilComercial>> GetAllAsync();
    Task<List<PerfilComercial>> GetActivosAsync();
}


