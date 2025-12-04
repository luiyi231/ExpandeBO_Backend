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

        // Validar precio
        if (producto.Precio <= 0)
        {
            throw new ArgumentException("El precio debe ser mayor a 0");
        }
        if (producto.Precio > 999999)
        {
            throw new ArgumentException("El precio no puede ser mayor a 999999");
        }

        // Validar stock
        if (producto.Stock < 0)
        {
            throw new ArgumentException("El stock no puede ser negativo");
        }
        if (producto.Stock > 99999)
        {
            throw new ArgumentException("El stock no puede ser mayor a 99999");
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

        // Validar precio
        if (producto.Precio <= 0)
        {
            throw new ArgumentException("El precio debe ser mayor a 0");
        }
        if (producto.Precio > 999999)
        {
            throw new ArgumentException("El precio no puede ser mayor a 999999");
        }

        // Validar stock
        if (producto.Stock < 0)
        {
            throw new ArgumentException("El stock no puede ser negativo");
        }
        if (producto.Stock > 99999)
        {
            throw new ArgumentException("El stock no puede ser mayor a 99999");
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

    public async Task<List<Producto>> GetProductosByCiudadAsync(string ciudadId)
    {
        // Obtener todos los perfiles comerciales activos de la ciudad
        var perfiles = await _perfilComercialRepository.GetByCiudadIdAsync(ciudadId);
        
        if (perfiles == null || perfiles.Count == 0)
        {
            return new List<Producto>();
        }

        // Obtener los IDs de los perfiles
        var perfilesIds = perfiles.Select(p => p.Id!).ToList();

        // Obtener productos de esos perfiles que estén disponibles
        var productos = await _productoRepository.GetByPerfilesComercialesAsync(perfilesIds);
        
        // Filtrar solo los disponibles
        return productos.Where(p => p.Disponible && p.Stock > 0).ToList();
    }

    public async Task<List<Producto>> GetTodosLosProductosAsync()
    {
        return await _productoRepository.GetAllAsync();
    }

    public async Task<List<Producto>> GetTodosLosProductosAsync(int page, int pageSize)
    {
        return await _productoRepository.GetPaginadosAsync(page, pageSize);
    }

    public async Task<long> GetTotalProductosAsync()
    {
        return await _productoRepository.GetTotalCountAsync();
    }
}


