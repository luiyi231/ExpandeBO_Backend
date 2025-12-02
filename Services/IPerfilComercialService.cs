using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Services;

public interface IPerfilComercialService
{
    Task<PerfilComercial> CreatePerfilAsync(PerfilComercial perfil, string empresaId);
    Task<PerfilComercial> UpdatePerfilAsync(string perfilId, PerfilComercial perfil, string empresaId);
    Task<bool> DeletePerfilAsync(string perfilId, string empresaId);
    Task<PerfilComercial?> GetPerfilByIdAsync(string id);
    Task<List<PerfilComercial>> GetPerfilesByEmpresaAsync(string empresaId);
    Task<List<PerfilComercial>> GetPerfilesActivosAsync();
}


