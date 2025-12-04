using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IPerfilComercialRepository _perfilComercialRepository;
    private readonly IPedidoProductoRepository _pedidoProductoRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProductoRepository productoRepository,
        IPerfilComercialRepository perfilComercialRepository,
        IPedidoProductoRepository pedidoProductoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _productoRepository = productoRepository;
        _perfilComercialRepository = perfilComercialRepository;
        _pedidoProductoRepository = pedidoProductoRepository;
    }

    public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
    {
        // Validar que el perfil comercial existe
        var perfil = await _perfilComercialRepository.GetByIdAsync(pedido.PerfilComercialId);
        if (perfil == null || !perfil.Activo)
        {
            throw new InvalidOperationException("Perfil comercial no encontrado o inactivo");
        }

        // Validar productos y calcular totales
        decimal subtotal = 0;
        foreach (var item in pedido.Items)
        {
            var producto = await _productoRepository.GetByIdAsync(item.ProductoId);
            if (producto == null || !producto.Disponible || producto.Stock < item.Cantidad)
            {
                throw new InvalidOperationException($"Producto {item.NombreProducto} no disponible o sin stock suficiente");
            }

            if (producto.PerfilComercialId != pedido.PerfilComercialId)
            {
                throw new InvalidOperationException($"El producto {item.NombreProducto} no pertenece a este perfil comercial");
            }

            item.PrecioUnitario = producto.Precio;
            item.Subtotal = producto.Precio * item.Cantidad;
            subtotal += item.Subtotal;
        }

        pedido.Subtotal = subtotal;
        pedido.Total = subtotal; // Por ahora sin impuestos ni descuentos
        pedido.Estado = "Pendiente";
        pedido.FechaCreacion = DateTime.UtcNow;

        // Crear el pedido
        var pedidoCreado = await _pedidoRepository.CreateAsync(pedido);

        // Crear registros en la tabla intermedia (muchos a muchos) y actualizar stock
        foreach (var item in pedido.Items)
        {
            var pedidoProducto = new PedidoProducto
            {
                PedidoId = pedidoCreado.Id!,
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Subtotal = item.Subtotal,
                FechaCreacion = DateTime.UtcNow
            };
            await _pedidoProductoRepository.CreateAsync(pedidoProducto);

            // Actualizar stock del producto
            var producto = await _productoRepository.GetByIdAsync(item.ProductoId);
            if (producto != null)
            {
                producto.Stock -= item.Cantidad;

                // Si el stock llega a 0 o menos, marcar como no disponible
                if (producto.Stock <= 0)
                {
                    producto.Stock = 0;
                    producto.Disponible = false;
                }

                await _productoRepository.UpdateAsync(producto);
            }
        }

        return pedidoCreado;
    }

    public async Task<Pedido> UpdatePedidoEstadoAsync(string pedidoId, string nuevoEstado, string? empresaId = null)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
        if (pedido == null)
        {
            throw new KeyNotFoundException("Pedido no encontrado");
        }

        // Si se proporciona empresaId, validar que el pedido pertenece a esa empresa
        if (!string.IsNullOrEmpty(empresaId))
        {
            var perfil = await _perfilComercialRepository.GetByIdAsync(pedido.PerfilComercialId);
            if (perfil == null || perfil.EmpresaId != empresaId)
            {
                throw new UnauthorizedAccessException("No tienes permiso para actualizar este pedido");
            }
        }

        var estadosValidos = new[] { "Pendiente", "Confirmado", "EnPreparacion", "EnCamino", "Entregado", "Cancelado" };
        if (!estadosValidos.Contains(nuevoEstado))
        {
            throw new ArgumentException("Estado inválido");
        }

        // Validar que no se puede cancelar un pedido entregado
        if (nuevoEstado == "Cancelado" && pedido.Estado == "Entregado")
        {
            throw new InvalidOperationException("No se puede cancelar un pedido que ya ha sido entregado");
        }

        var estadoAnterior = pedido.Estado;
        pedido.Estado = nuevoEstado;
        pedido.FechaActualizacion = DateTime.UtcNow;

        // Si se cancela el pedido y no estaba cancelado antes, restaurar el stock
        // Solo se puede cancelar si el estado anterior era Pendiente, Confirmado, EnPreparacion o EnCamino
        if (nuevoEstado == "Cancelado" && estadoAnterior != "Cancelado" && estadoAnterior != "Entregado" && pedido.Items != null)
        {
            foreach (var item in pedido.Items)
            {
                var producto = await _productoRepository.GetByIdAsync(item.ProductoId);
                if (producto != null)
                {
                    producto.Stock += item.Cantidad;

                    // Si el stock se restaura y era 0, marcar como disponible nuevamente
                    if (producto.Stock > 0)
                    {
                        producto.Disponible = true;
                    }

                    await _productoRepository.UpdateAsync(producto);
                }
            }
        }

        return await _pedidoRepository.UpdateAsync(pedido);
    }

    public async Task<Pedido?> GetPedidoByIdAsync(string id)
    {
        return await _pedidoRepository.GetByIdAsync(id);
    }

    public async Task<List<Pedido>> GetPedidosByClienteAsync(string clienteId)
    {
        return await _pedidoRepository.GetByClienteIdAsync(clienteId);
    }

    public async Task<List<Pedido>> GetPedidosByPerfilComercialAsync(string perfilComercialId)
    {
        return await _pedidoRepository.GetByPerfilComercialIdAsync(perfilComercialId);
    }

    public async Task<List<Pedido>> GetAllPedidosAsync()
    {
        return await _pedidoRepository.GetAllAsync();
    }

    public async Task<Pedido> UpdateRepartidorPosicionAsync(
        string pedidoId,
        double? latitud,
        double? longitud,
        double? progresoRuta,
        string? rutaGeometry = null,
        int? duracionRutaSegundos = null)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
        if (pedido == null)
        {
            throw new KeyNotFoundException("Pedido no encontrado");
        }

        // Actualizar posición del repartidor
        if (latitud.HasValue)
        {
            pedido.LatitudRepartidor = latitud.Value;
        }
        if (longitud.HasValue)
        {
            pedido.LongitudRepartidor = longitud.Value;
        }
        if (progresoRuta.HasValue)
        {
            pedido.ProgresoRuta = progresoRuta.Value;
        }
        if (!string.IsNullOrEmpty(rutaGeometry))
        {
            pedido.RutaGeometry = rutaGeometry;
            // Si es la primera vez que se guarda la ruta, guardar también la fecha de inicio
            if (!pedido.FechaInicioRuta.HasValue)
            {
                pedido.FechaInicioRuta = DateTime.UtcNow;
            }
        }
        if (duracionRutaSegundos.HasValue)
        {
            pedido.DuracionRutaSegundos = duracionRutaSegundos.Value;
        }

        pedido.FechaActualizacion = DateTime.UtcNow;

        return await _pedidoRepository.UpdateAsync(pedido);
    }
}

