using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IPedidoProductoRepository
{
    Task<PedidoProducto?> GetByIdAsync(string id);
    Task<List<PedidoProducto>> GetByPedidoIdAsync(string pedidoId);
    Task<List<PedidoProducto>> GetByProductoIdAsync(string productoId);
    Task<PedidoProducto> CreateAsync(PedidoProducto pedidoProducto);
    Task<PedidoProducto> UpdateAsync(PedidoProducto pedidoProducto);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteByPedidoIdAsync(string pedidoId);
    Task<List<PedidoProducto>> GetAllAsync();
}

