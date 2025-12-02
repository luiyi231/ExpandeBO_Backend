using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IPedidoRepository
{
    Task<Pedido?> GetByIdAsync(string id);
    Task<List<Pedido>> GetByClienteIdAsync(string clienteId);
    Task<List<Pedido>> GetByPerfilComercialIdAsync(string perfilComercialId);
    Task<Pedido> CreateAsync(Pedido pedido);
    Task<Pedido> UpdateAsync(Pedido pedido);
    Task<bool> DeleteAsync(string id);
    Task<List<Pedido>> GetAllAsync();
}


