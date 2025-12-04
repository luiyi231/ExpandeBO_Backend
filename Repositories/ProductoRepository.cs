using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly IMongoCollection<Producto> _productos;

    public ProductoRepository(IMongoContext context)
    {
        _productos = context.Database.GetCollection<Producto>("productos");
    }

    public async Task<Producto?> GetByIdAsync(string id)
    {
        return await _productos.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Producto>> GetByPerfilComercialIdAsync(string perfilComercialId)
    {
        return await _productos.Find(p => p.PerfilComercialId == perfilComercialId).ToListAsync();
    }

    public async Task<List<Producto>> GetByCategoriaIdAsync(string categoriaId)
    {
        return await _productos.Find(p => p.CategoriaId == categoriaId).ToListAsync();
    }

    public async Task<List<Producto>> GetBySubcategoriaIdAsync(string subcategoriaId)
    {
        return await _productos.Find(p => p.SubcategoriaId == subcategoriaId).ToListAsync();
    }

    public async Task<Producto> CreateAsync(Producto producto)
    {
        await _productos.InsertOneAsync(producto);
        return producto;
    }

    public async Task<Producto> UpdateAsync(Producto producto)
    {
        producto.FechaUltimaActualizacion = DateTime.UtcNow;
        await _productos.ReplaceOneAsync(p => p.Id == producto.Id, producto);
        return producto;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _productos.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Producto>> GetAllAsync()
    {
        return await _productos.Find(_ => true).ToListAsync();
    }

    public async Task<List<Producto>> GetDisponiblesAsync()
    {
        return await _productos.Find(p => p.Disponible && p.Stock > 0).ToListAsync();
    }

    public async Task<List<Producto>> GetByPerfilesComercialesAsync(List<string> perfilesComercialesIds)
    {
        var filter = Builders<Producto>.Filter.In(p => p.PerfilComercialId, perfilesComercialesIds);
        return await _productos.Find(filter).ToListAsync();
    }

    public async Task<List<Producto>> GetPaginadosAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return await _productos.Find(_ => true)
            .SortByDescending(p => p.FechaCreacion)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> GetTotalCountAsync()
    {
        return await _productos.CountDocumentsAsync(_ => true);
    }

    public async Task<List<Producto>> GetPaginadosConFiltrosAsync(int page, int pageSize, List<string>? perfilesIds, string? categoriaId, string? subcategoriaId)
    {
        var skip = (page - 1) * pageSize;
        var filterBuilder = Builders<Producto>.Filter;
        var filters = new List<FilterDefinition<Producto>>();

        if (perfilesIds != null && perfilesIds.Count > 0)
        {
            filters.Add(filterBuilder.In(p => p.PerfilComercialId, perfilesIds));
        }

        if (!string.IsNullOrEmpty(categoriaId))
        {
            filters.Add(filterBuilder.Eq(p => p.CategoriaId, categoriaId));
        }

        if (!string.IsNullOrEmpty(subcategoriaId))
        {
            filters.Add(filterBuilder.Eq(p => p.SubcategoriaId, subcategoriaId));
        }

        var combinedFilter = filters.Count > 0 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;

        return await _productos.Find(combinedFilter)
            .SortByDescending(p => p.FechaCreacion)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> GetTotalCountConFiltrosAsync(List<string>? perfilesIds, string? categoriaId, string? subcategoriaId)
    {
        var filterBuilder = Builders<Producto>.Filter;
        var filters = new List<FilterDefinition<Producto>>();

        if (perfilesIds != null && perfilesIds.Count > 0)
        {
            filters.Add(filterBuilder.In(p => p.PerfilComercialId, perfilesIds));
        }

        if (!string.IsNullOrEmpty(categoriaId))
        {
            filters.Add(filterBuilder.Eq(p => p.CategoriaId, categoriaId));
        }

        if (!string.IsNullOrEmpty(subcategoriaId))
        {
            filters.Add(filterBuilder.Eq(p => p.SubcategoriaId, subcategoriaId));
        }

        var combinedFilter = filters.Count > 0 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;

        return await _productos.CountDocumentsAsync(combinedFilter);
    }
}


