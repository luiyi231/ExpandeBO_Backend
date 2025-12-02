using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _usuarios;

    public UsuarioRepository(IMongoContext context)
    {
        _usuarios = context.Database.GetCollection<Usuario>("usuarios");
    }

    public async Task<Usuario?> GetByIdAsync(string id)
    {
        return await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _usuarios.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        await _usuarios.InsertOneAsync(usuario);
        return usuario;
    }

    public async Task<Usuario> UpdateAsync(Usuario usuario)
    {
        usuario.FechaUltimaActualizacion = DateTime.UtcNow;
        await _usuarios.ReplaceOneAsync(u => u.Id == usuario.Id, usuario);
        return usuario;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _usuarios.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        return await _usuarios.Find(_ => true).ToListAsync();
    }
}


