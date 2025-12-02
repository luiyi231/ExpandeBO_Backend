using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface ISuscripcionEmpresaRepository
{
    Task<SuscripcionEmpresa?> GetByIdAsync(string id);
    Task<SuscripcionEmpresa?> GetActivaByEmpresaIdAsync(string empresaId);
    Task<SuscripcionEmpresa> CreateAsync(SuscripcionEmpresa suscripcion);
    Task<SuscripcionEmpresa> UpdateAsync(SuscripcionEmpresa suscripcion);
    Task<bool> DeleteAsync(string id);
    Task<List<SuscripcionEmpresa>> GetByEmpresaIdAsync(string empresaId);
}


