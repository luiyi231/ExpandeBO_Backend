using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IProductoRepository
{
    Task<Producto?> GetByIdAsync(string id);
    Task<List<Producto>> GetByPerfilComercialIdAsync(string perfilComercialId);
    Task<List<Producto>> GetByCategoriaIdAsync(string categoriaId);
    Task<List<Producto>> GetBySubcategoriaIdAsync(string subcategoriaId);
    Task<List<Producto>> GetByPerfilesComercialesAsync(List<string> perfilesComercialesIds);
    Task<Producto> CreateAsync(Producto producto);
    Task<Producto> UpdateAsync(Producto producto);
    Task<bool> DeleteAsync(string id);
    Task<List<Producto>> GetAllAsync();
    Task<List<Producto>> GetDisponiblesAsync();
}


