using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IEmpresaRepository
{
    Task<Empresa?> GetByIdAsync(string id);
    Task<Empresa?> GetByUsuarioIdAsync(string usuarioId);
    Task<Empresa> CreateAsync(Empresa empresa);
    Task<Empresa> UpdateAsync(Empresa empresa);
    Task<bool> DeleteAsync(string id);
    Task<List<Empresa>> GetAllAsync();
}


