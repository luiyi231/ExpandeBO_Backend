using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IMongoCollection<Cliente> _clientes;

    public ClienteRepository(IMongoContext context)
    {
        _clientes = context.Database.GetCollection<Cliente>("clientes");
    }

    public async Task<Cliente?> GetByIdAsync(string id)
    {
        return await _clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Cliente?> GetByUsuarioIdAsync(string usuarioId)
    {
        return await _clientes.Find(c => c.UsuarioId == usuarioId).FirstOrDefaultAsync();
    }

    public async Task<List<Cliente>> GetByCiudadIdAsync(string ciudadId)
    {
        return await _clientes.Find(c => c.CiudadId == ciudadId && c.Activo).ToListAsync();
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        await _clientes.InsertOneAsync(cliente);
        return cliente;
    }

    public async Task<Cliente> UpdateAsync(Cliente cliente)
    {
        cliente.FechaUltimaActualizacion = DateTime.UtcNow;
        await _clientes.ReplaceOneAsync(c => c.Id == cliente.Id, cliente);
        return cliente;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _clientes.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _clientes.Find(_ => true).ToListAsync();
    }
}

