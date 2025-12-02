using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Services;
using ExpandeBO_Backend.Repositories;
using System.Security.Claims;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPerfilComercialService _perfilComercialService;

    public ChatController(
        IChatService chatService,
        IUsuarioRepository usuarioRepository,
        IEmpresaRepository empresaRepository,
        IPerfilComercialService perfilComercialService)
    {
        _chatService = chatService;
        _usuarioRepository = usuarioRepository;
        _empresaRepository = empresaRepository;
        _perfilComercialService = perfilComercialService;
    }

    [HttpPost("crear-o-obtener")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Chat>> CrearObtenerChat([FromBody] CrearChatRequest request)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol == "Cliente")
            {
                var chat = await _chatService.CrearObtenerChatAsync(usuarioId!, request.PerfilComercialId, request.EsSoporte);
                return Ok(chat);
            }

            return BadRequest(new { message = "Solo los clientes pueden crear chats" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost("{chatId}/mensaje")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Chat>> EnviarMensaje(string chatId, [FromBody] EnviarMensajeRequest request)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId!);
            if (usuario == null)
            {
                return Unauthorized();
            }

            string remitenteNombre = $"{usuario.Nombre} {usuario.Apellido}";

            var chat = await _chatService.EnviarMensajeAsync(chatId, usuarioId!, remitenteNombre, rol!, request.Contenido);
            return Ok(chat);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("mis-chats")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<List<Chat>>> GetMisChats()
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol == "Cliente")
            {
                var chats = await _chatService.GetChatsByClienteAsync(usuarioId!);
                return Ok(chats);
            }

            if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var perfiles = await _perfilComercialService.GetPerfilesByEmpresaAsync(empresa.Id!);
                var todosChats = new List<Chat>();

                foreach (var perfil in perfiles)
                {
                    var chats = await _chatService.GetChatsByPerfilComercialAsync(perfil.Id!);
                    todosChats.AddRange(chats);
                }

                return Ok(todosChats);
            }

            // Admin - chats de soporte
            var chatsSoporte = await _chatService.GetChatsSoporteAsync();
            return Ok(chatsSoporte);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Chat>> GetChat(string id)
    {
        try
        {
            var chat = await _chatService.GetChatByIdAsync(id);
            if (chat == null)
            {
                return NotFound(new { message = "Chat no encontrado" });
            }

            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            // Validar acceso
            if (rol == "Cliente" && chat.ClienteId != usuarioId)
            {
                return Forbid("No tienes permiso para ver este chat");
            }

            if (rol == "Empresa" && !chat.EsSoporte)
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var perfil = await _perfilComercialService.GetPerfilByIdAsync(chat.PerfilComercialId!);
                if (perfil == null || perfil.EmpresaId != empresa.Id)
                {
                    return Forbid("No tienes permiso para ver este chat");
                }
            }

            return Ok(chat);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

public class CrearChatRequest
{
    public string? PerfilComercialId { get; set; }
    public bool EsSoporte { get; set; } = false;
}

public class EnviarMensajeRequest
{
    public string Contenido { get; set; } = string.Empty;
}


