using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Services;

public class EmpresaEliminacionService : IEmpresaEliminacionService
{
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPerfilComercialRepository _perfilComercialRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ISuscripcionEmpresaRepository _suscripcionEmpresaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EmpresaEliminacionService(
        IEmpresaRepository empresaRepository,
        IPerfilComercialRepository perfilComercialRepository,
        IProductoRepository productoRepository,
        IPedidoRepository pedidoRepository,
        ISuscripcionEmpresaRepository suscripcionEmpresaRepository,
        IUsuarioRepository usuarioRepository)
    {
        _empresaRepository = empresaRepository;
        _perfilComercialRepository = perfilComercialRepository;
        _productoRepository = productoRepository;
        _pedidoRepository = pedidoRepository;
        _suscripcionEmpresaRepository = suscripcionEmpresaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<bool> EliminarEmpresaConRelacionesAsync(string empresaId)
    {
        // 1. Verificar que la empresa existe
        var empresa = await _empresaRepository.GetByIdAsync(empresaId);
        if (empresa == null)
        {
            return false;
        }

        try
        {
            // 2. Obtener todos los perfiles comerciales de la empresa
            var perfiles = await _perfilComercialRepository.GetByEmpresaIdAsync(empresaId);

            // 3. Para cada perfil comercial, eliminar productos y pedidos
            foreach (var perfil in perfiles)
            {
                if (perfil.Id == null) continue;

                // 3.1. Obtener y eliminar productos del perfil
                var productos = await _productoRepository.GetByPerfilComercialIdAsync(perfil.Id);
                foreach (var producto in productos)
                {
                    if (producto.Id != null)
                    {
                        await _productoRepository.DeleteAsync(producto.Id);
                    }
                }

                // 3.2. Obtener y eliminar pedidos del perfil
                var pedidos = await _pedidoRepository.GetByPerfilComercialIdAsync(perfil.Id);
                foreach (var pedido in pedidos)
                {
                    if (pedido.Id != null)
                    {
                        await _pedidoRepository.DeleteAsync(pedido.Id);
                    }
                }

                // 3.3. Eliminar el perfil comercial
                await _perfilComercialRepository.DeleteAsync(perfil.Id);
            }

            // 4. Eliminar todas las suscripciones de la empresa
            var suscripciones = await _suscripcionEmpresaRepository.GetByEmpresaIdAsync(empresaId);
            foreach (var suscripcion in suscripciones)
            {
                if (suscripcion.Id != null)
                {
                    await _suscripcionEmpresaRepository.DeleteAsync(suscripcion.Id);
                }
            }

            // 5. Eliminar el usuario asociado a la empresa
            if (!string.IsNullOrEmpty(empresa.UsuarioId))
            {
                await _usuarioRepository.DeleteAsync(empresa.UsuarioId);
            }

            // 6. Finalmente, eliminar la empresa
            return await _empresaRepository.DeleteAsync(empresaId);
        }
        catch (Exception)
        {
            // Si algo falla, retornar false
            return false;
        }
    }
}

