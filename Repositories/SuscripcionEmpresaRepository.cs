using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class SuscripcionEmpresaRepository : ISuscripcionEmpresaRepository
{
    private readonly IMongoCollection<SuscripcionEmpresa> _suscripciones;

    public SuscripcionEmpresaRepository(IMongoContext context)
    {
        _suscripciones = context.Database.GetCollection<SuscripcionEmpresa>("suscripciones_empresa");
    }

    public async Task<SuscripcionEmpresa?> GetByIdAsync(string id)
    {
        return await _suscripciones.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<SuscripcionEmpresa?> GetActivaByEmpresaIdAsync(string empresaId)
    {
        var ahora = DateTime.UtcNow;
        return await _suscripciones.Find(s => s.EmpresaId == empresaId && s.Activa && s.FechaFin >= ahora).FirstOrDefaultAsync();
    }

    public async Task<SuscripcionEmpresa> CreateAsync(SuscripcionEmpresa suscripcion)
    {
        await _suscripciones.InsertOneAsync(suscripcion);
        return suscripcion;
    }

    public async Task<SuscripcionEmpresa> UpdateAsync(SuscripcionEmpresa suscripcion)
    {
        await _suscripciones.ReplaceOneAsync(s => s.Id == suscripcion.Id, suscripcion);
        return suscripcion;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _suscripciones.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<SuscripcionEmpresa>> GetByEmpresaIdAsync(string empresaId)
    {
        return await _suscripciones.Find(s => s.EmpresaId == empresaId).ToListAsync();
    }
}


