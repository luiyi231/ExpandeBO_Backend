using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Services;

public interface IProductoService
{
    Task<Producto> CreateProductoAsync(Producto producto, string empresaId);
    Task<Producto> UpdateProductoAsync(string productoId, Producto producto, string empresaId);
    Task<bool> DeleteProductoAsync(string productoId, string empresaId);
    Task<Producto?> GetProductoByIdAsync(string id);
    Task<List<Producto>> GetProductosByPerfilAsync(string perfilComercialId);
    Task<List<Producto>> GetProductosByCategoriaAsync(string categoriaId);
    Task<List<Producto>> GetProductosDisponiblesAsync();
    Task<List<Producto>> GetProductosByCiudadAsync(string ciudadId);
    Task<List<Producto>> GetTodosLosProductosAsync();
}


