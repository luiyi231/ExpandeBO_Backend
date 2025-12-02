using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class PedidoProductoRepository : IPedidoProductoRepository
{
    private readonly IMongoCollection<PedidoProducto> _pedidoProductos;

    public PedidoProductoRepository(IMongoContext context)
    {
        _pedidoProductos = context.Database.GetCollection<PedidoProducto>("pedido_productos");
    }

    public async Task<PedidoProducto?> GetByIdAsync(string id)
    {
        return await _pedidoProductos.Find(pp => pp.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<PedidoProducto>> GetByPedidoIdAsync(string pedidoId)
    {
        return await _pedidoProductos.Find(pp => pp.PedidoId == pedidoId).ToListAsync();
    }

    public async Task<List<PedidoProducto>> GetByProductoIdAsync(string productoId)
    {
        return await _pedidoProductos.Find(pp => pp.ProductoId == productoId).ToListAsync();
    }

    public async Task<PedidoProducto> CreateAsync(PedidoProducto pedidoProducto)
    {
        await _pedidoProductos.InsertOneAsync(pedidoProducto);
        return pedidoProducto;
    }

    public async Task<PedidoProducto> UpdateAsync(PedidoProducto pedidoProducto)
    {
        await _pedidoProductos.ReplaceOneAsync(pp => pp.Id == pedidoProducto.Id, pedidoProducto);
        return pedidoProducto;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _pedidoProductos.DeleteOneAsync(pp => pp.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteByPedidoIdAsync(string pedidoId)
    {
        var result = await _pedidoProductos.DeleteManyAsync(pp => pp.PedidoId == pedidoId);
        return result.DeletedCount > 0;
    }

    public async Task<List<PedidoProducto>> GetAllAsync()
    {
        return await _pedidoProductos.Find(_ => true).ToListAsync();
    }
}

