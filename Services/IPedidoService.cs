using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Services;

public interface IPedidoService
{
    Task<Pedido> CreatePedidoAsync(Pedido pedido);
    Task<Pedido> UpdatePedidoEstadoAsync(string pedidoId, string nuevoEstado, string? empresaId = null);
    Task<Pedido> UpdateRepartidorPosicionAsync(
        string pedidoId,
        double? latitud,
        double? longitud,
        double? progresoRuta,
        string? rutaGeometry = null,
        int? duracionRutaSegundos = null);
    Task<Pedido> UpdatePuntuacionEntregaAsync(string pedidoId, int puntuacion);
    Task<Pedido?> GetPedidoByIdAsync(string id);
    Task<List<Pedido>> GetPedidosByClienteAsync(string clienteId);
    Task<List<Pedido>> GetPedidosByPerfilComercialAsync(string perfilComercialId);
    Task<List<Pedido>> GetAllPedidosAsync();
}

