using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<Chat> CrearObtenerChatAsync(string clienteId, string? perfilComercialId, bool esSoporte = false)
    {
        Chat? chat = null;

        if (esSoporte)
        {
            chat = await _chatRepository.GetSoporteByClienteAsync(clienteId);
        }
        else if (!string.IsNullOrEmpty(perfilComercialId))
        {
            chat = await _chatRepository.GetByClienteAndPerfilAsync(clienteId, perfilComercialId);
        }

        if (chat == null)
        {
            chat = new Chat
            {
                ClienteId = clienteId,
                PerfilComercialId = perfilComercialId,
                EsSoporte = esSoporte,
                Mensajes = new List<Mensaje>(),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            chat = await _chatRepository.CreateAsync(chat);
        }

        return chat;
    }

    public async Task<Chat> EnviarMensajeAsync(string chatId, string remitenteId, string remitenteNombre, string remitenteRol, string contenido)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null)
        {
            throw new KeyNotFoundException("Chat no encontrado");
        }

        var mensaje = new Mensaje
        {
            RemitenteId = remitenteId,
            RemitenteNombre = remitenteNombre,
            RemitenteRol = remitenteRol,
            Contenido = contenido,
            FechaEnvio = DateTime.UtcNow,
            Leido = false
        };

        chat.Mensajes.Add(mensaje);
        chat.FechaUltimaActualizacion = DateTime.UtcNow;

        return await _chatRepository.UpdateAsync(chat);
    }

    public async Task<Chat?> GetChatByIdAsync(string id)
    {
        return await _chatRepository.GetByIdAsync(id);
    }

    public async Task<List<Chat>> GetChatsByClienteAsync(string clienteId)
    {
        return await _chatRepository.GetByClienteIdAsync(clienteId);
    }

    public async Task<List<Chat>> GetChatsByPerfilComercialAsync(string perfilComercialId)
    {
        return await _chatRepository.GetByPerfilComercialIdAsync(perfilComercialId);
    }

    public async Task<List<Chat>> GetChatsSoporteAsync()
    {
        return await _chatRepository.GetSoporteAsync();
    }
}


