using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Services;

public interface IChatService
{
    Task<Chat> CrearObtenerChatAsync(string clienteId, string? perfilComercialId, bool esSoporte = false);
    Task<Chat> EnviarMensajeAsync(string chatId, string remitenteId, string remitenteNombre, string remitenteRol, string contenido);
    Task<Chat?> GetChatByIdAsync(string id);
    Task<List<Chat>> GetChatsByClienteAsync(string clienteId);
    Task<List<Chat>> GetChatsByPerfilComercialAsync(string perfilComercialId);
    Task<List<Chat>> GetChatsSoporteAsync();
}


