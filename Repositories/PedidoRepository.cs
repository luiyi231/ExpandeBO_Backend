using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly IMongoCollection<Pedido> _pedidos;

    public PedidoRepository(IMongoContext context)
    {
        _pedidos = context.Database.GetCollection<Pedido>("pedidos");
    }

    public async Task<Pedido?> GetByIdAsync(string id)
    {
        return await _pedidos.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Pedido>> GetByClienteIdAsync(string clienteId)
    {
        return await _pedidos.Find(p => p.ClienteId == clienteId).SortByDescending(p => p.FechaCreacion).ToListAsync();
    }

    public async Task<List<Pedido>> GetByPerfilComercialIdAsync(string perfilComercialId)
    {
        return await _pedidos.Find(p => p.PerfilComercialId == perfilComercialId).SortByDescending(p => p.FechaCreacion).ToListAsync();
    }

    public async Task<Pedido> CreateAsync(Pedido pedido)
    {
        await _pedidos.InsertOneAsync(pedido);
        return pedido;
    }

    public async Task<Pedido> UpdateAsync(Pedido pedido)
    {
        pedido.FechaActualizacion = DateTime.UtcNow;
        await _pedidos.ReplaceOneAsync(p => p.Id == pedido.Id, pedido);
        return pedido;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _pedidos.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Pedido>> GetAllAsync()
    {
        return await _pedidos.Find(_ => true).SortByDescending(p => p.FechaCreacion).ToListAsync();
    }
}


