using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly IPerfilComercialRepository _perfilComercialRepository;

    public ProductoService(
        IProductoRepository productoRepository,
        IPerfilComercialRepository perfilComercialRepository)
    {
        _productoRepository = productoRepository;
        _perfilComercialRepository = perfilComercialRepository;
    }

    public async Task<Producto> CreateProductoAsync(Producto producto, string empresaId)
    {
        var perfil = await _perfilComercialRepository.GetByIdAsync(producto.PerfilComercialId);
        if (perfil == null || perfil.EmpresaId != empresaId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para crear productos en este perfil comercial");
        }

        return await _productoRepository.CreateAsync(producto);
    }

    public async Task<Producto> UpdateProductoAsync(string productoId, Producto producto, string empresaId)
    {
        var productoExistente = await _productoRepository.GetByIdAsync(productoId);
        if (productoExistente == null)
        {
            throw new KeyNotFoundException("Producto no encontrado");
        }

        var perfil = await _perfilComercialRepository.GetByIdAsync(productoExistente.PerfilComercialId);
        if (perfil == null || perfil.EmpresaId != empresaId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para actualizar este producto");
        }

        producto.Id = productoId;
        producto.PerfilComercialId = productoExistente.PerfilComercialId;
        return await _productoRepository.UpdateAsync(producto);
    }

    public async Task<bool> DeleteProductoAsync(string productoId, string empresaId)
    {
        var producto = await _productoRepository.GetByIdAsync(productoId);
        if (producto == null)
        {
            return false;
        }

        var perfil = await _perfilComercialRepository.GetByIdAsync(producto.PerfilComercialId);
        if (perfil == null || perfil.EmpresaId != empresaId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para eliminar este producto");
        }

        return await _productoRepository.DeleteAsync(productoId);
    }

    public async Task<Producto?> GetProductoByIdAsync(string id)
    {
        return await _productoRepository.GetByIdAsync(id);
    }

    public async Task<List<Producto>> GetProductosByPerfilAsync(string perfilComercialId)
    {
        return await _productoRepository.GetByPerfilComercialIdAsync(perfilComercialId);
    }

    public async Task<List<Producto>> GetProductosByCategoriaAsync(string categoriaId)
    {
        return await _productoRepository.GetByCategoriaIdAsync(categoriaId);
    }

    public async Task<List<Producto>> GetProductosDisponiblesAsync()
    {
        return await _productoRepository.GetDisponiblesAsync();
    }
}


