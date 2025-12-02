using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class PerfilComercialRepository : IPerfilComercialRepository
{
    private readonly IMongoCollection<PerfilComercial> _perfiles;

    public PerfilComercialRepository(IMongoContext context)
    {
        _perfiles = context.Database.GetCollection<PerfilComercial>("perfiles_comerciales");
    }

    public async Task<PerfilComercial?> GetByIdAsync(string id)
    {
        return await _perfiles.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<PerfilComercial>> GetByEmpresaIdAsync(string empresaId)
    {
        return await _perfiles.Find(p => p.EmpresaId == empresaId).ToListAsync();
    }

    public async Task<PerfilComercial> CreateAsync(PerfilComercial perfil)
    {
        await _perfiles.InsertOneAsync(perfil);
        return perfil;
    }

    public async Task<PerfilComercial> UpdateAsync(PerfilComercial perfil)
    {
        perfil.FechaUltimaActualizacion = DateTime.UtcNow;
        await _perfiles.ReplaceOneAsync(p => p.Id == perfil.Id, perfil);
        return perfil;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _perfiles.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<PerfilComercial>> GetAllAsync()
    {
        return await _perfiles.Find(_ => true).ToListAsync();
    }

    public async Task<List<PerfilComercial>> GetActivosAsync()
    {
        return await _perfiles.Find(p => p.Activo).ToListAsync();
    }
}


