using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<Chat> _chats;

    public ChatRepository(IMongoContext context)
    {
        _chats = context.Database.GetCollection<Chat>("chats");
    }

    public async Task<Chat?> GetByIdAsync(string id)
    {
        return await _chats.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Chat?> GetByClienteAndPerfilAsync(string clienteId, string perfilComercialId)
    {
        return await _chats.Find(c => c.ClienteId == clienteId && c.PerfilComercialId == perfilComercialId && !c.EsSoporte).FirstOrDefaultAsync();
    }

    public async Task<Chat?> GetSoporteByClienteAsync(string clienteId)
    {
        return await _chats.Find(c => c.ClienteId == clienteId && c.EsSoporte).FirstOrDefaultAsync();
    }

    public async Task<List<Chat>> GetByClienteIdAsync(string clienteId)
    {
        return await _chats.Find(c => c.ClienteId == clienteId && c.Activo).SortByDescending(c => c.FechaUltimaActualizacion).ToListAsync();
    }

    public async Task<List<Chat>> GetByPerfilComercialIdAsync(string perfilComercialId)
    {
        return await _chats.Find(c => c.PerfilComercialId == perfilComercialId && c.Activo).SortByDescending(c => c.FechaUltimaActualizacion).ToListAsync();
    }

    public async Task<List<Chat>> GetSoporteAsync()
    {
        return await _chats.Find(c => c.EsSoporte && c.Activo).SortByDescending(c => c.FechaUltimaActualizacion).ToListAsync();
    }

    public async Task<Chat> CreateAsync(Chat chat)
    {
        await _chats.InsertOneAsync(chat);
        return chat;
    }

    public async Task<Chat> UpdateAsync(Chat chat)
    {
        chat.FechaUltimaActualizacion = DateTime.UtcNow;
        await _chats.ReplaceOneAsync(c => c.Id == chat.Id, chat);
        return chat;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _chats.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }
}


