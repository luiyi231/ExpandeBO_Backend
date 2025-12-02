using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly IMongoCollection<Empresa> _empresas;

    public EmpresaRepository(IMongoContext context)
    {
        _empresas = context.Database.GetCollection<Empresa>("empresas");
    }

    public async Task<Empresa?> GetByIdAsync(string id)
    {
        return await _empresas.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Empresa?> GetByUsuarioIdAsync(string usuarioId)
    {
        return await _empresas.Find(e => e.UsuarioId == usuarioId).FirstOrDefaultAsync();
    }

    public async Task<Empresa> CreateAsync(Empresa empresa)
    {
        await _empresas.InsertOneAsync(empresa);
        return empresa;
    }

    public async Task<Empresa> UpdateAsync(Empresa empresa)
    {
        empresa.FechaUltimaActualizacion = DateTime.UtcNow;
        await _empresas.ReplaceOneAsync(e => e.Id == empresa.Id, empresa);
        return empresa;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _empresas.DeleteOneAsync(e => e.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Empresa>> GetAllAsync()
    {
        return await _empresas.Find(_ => true).ToListAsync();
    }
}


