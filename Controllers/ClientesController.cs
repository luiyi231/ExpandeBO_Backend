using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Services;
using System.Security.Claims;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet("mi-perfil")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Cliente>> GetMiPerfil()
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var rol = User.FindFirst("Rol")?.Value;
            if (rol != "Cliente")
            {
                return Forbid("Solo los clientes pueden acceder a este endpoint");
            }

            var cliente = await _clienteService.GetClienteByUsuarioIdAsync(usuarioId);
            if (cliente == null)
            {
                return NotFound(new { message = "Perfil de cliente no encontrado" });
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("mi-perfil")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Cliente>> UpdateMiPerfil([FromBody] UpdateClienteRequest request)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var rol = User.FindFirst("Rol")?.Value;
            if (rol != "Cliente")
            {
                return Forbid("Solo los clientes pueden actualizar su perfil");
            }

            var cliente = await _clienteService.GetClienteByUsuarioIdAsync(usuarioId);
            if (cliente == null)
            {
                return NotFound(new { message = "Perfil de cliente no encontrado" });
            }

            var clienteActualizado = await _clienteService.UpdateClienteAsync(cliente.Id!, request.CiudadId, request.Direccion);
            return Ok(clienteActualizado);
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

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Cliente>> GetCliente(string id)
    {
        try
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("ciudad/{ciudadId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<Cliente>>> GetClientesByCiudad(string ciudadId)
    {
        try
        {
            var clientes = await _clienteService.GetClientesByCiudadAsync(ciudadId);
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

public class UpdateClienteRequest
{
    public string CiudadId { get; set; } = string.Empty;
    public string? Direccion { get; set; }
}

