using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Models.DTOs;
using ExpandeBO_Backend.Services;
using ExpandeBO_Backend.Repositories;
using System.Security.Claims;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPerfilComercialService _perfilComercialService;

    public PedidosController(
        IPedidoService pedidoService,
        IEmpresaRepository empresaRepository,
        IPerfilComercialService perfilComercialService)
    {
        _pedidoService = pedidoService;
        _empresaRepository = empresaRepository;
        _perfilComercialService = perfilComercialService;
    }

    [HttpPost]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Pedido>> CreatePedido([FromBody] Pedido pedido)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol != "Cliente" && rol != "Administrador")
            {
                return Forbid("Solo los clientes pueden crear pedidos");
            }

            if (rol == "Cliente")
            {
                pedido.ClienteId = usuarioId!;
            }

            var nuevoPedido = await _pedidoService.CreatePedidoAsync(pedido);
            return CreatedAtAction(nameof(GetPedido), new { id = nuevoPedido.Id }, nuevoPedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Pedido>> GetPedido(string id)
    {
        try
        {
            var pedido = await _pedidoService.GetPedidoByIdAsync(id);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            // Validar acceso
            if (rol == "Cliente" && pedido.ClienteId != usuarioId)
            {
                return Forbid("No tienes permiso para ver este pedido");
            }

            if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var perfil = await _perfilComercialService.GetPerfilByIdAsync(pedido.PerfilComercialId);
                if (perfil == null || perfil.EmpresaId != empresa.Id)
                {
                    return Forbid("No tienes permiso para ver este pedido");
                }
            }

            return Ok(pedido);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("mis-pedidos")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<List<Pedido>>> GetMisPedidos()
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol == "Cliente")
            {
                var pedidos = await _pedidoService.GetPedidosByClienteAsync(usuarioId!);
                return Ok(pedidos);
            }

            if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var perfiles = await _perfilComercialService.GetPerfilesByEmpresaAsync(empresa.Id!);
                var todosPedidos = new List<Pedido>();

                foreach (var perfil in perfiles)
                {
                    var pedidos = await _pedidoService.GetPedidosByPerfilComercialAsync(perfil.Id!);
                    todosPedidos.AddRange(pedidos);
                }

                return Ok(todosPedidos);
            }

            // Admin - todos los pedidos
            var todosLosPedidos = await _pedidoService.GetAllPedidosAsync();
            return Ok(todosLosPedidos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("perfil/{idPerfil}")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<List<Pedido>>> GetPedidosByPerfil(string idPerfil)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var perfil = await _perfilComercialService.GetPerfilByIdAsync(idPerfil);
                if (perfil == null || perfil.EmpresaId != empresa.Id)
                {
                    return Forbid("No tienes permiso para ver los pedidos de este perfil");
                }
            }

            var pedidos = await _pedidoService.GetPedidosByPerfilComercialAsync(idPerfil);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}/estado")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Pedido>> UpdateEstado(string id, [FromBody] UpdateEstadoRequest request)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            // Obtener el pedido para validar permisos
            var pedido = await _pedidoService.GetPedidoByIdAsync(id);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            // Validar permisos seg�n el rol
            if (rol == "Cliente")
            {
                // Los clientes solo pueden actualizar sus propios pedidos
                if (pedido.ClienteId != usuarioId)
                {
                    return Forbid("No tienes permiso para actualizar este pedido");
                }
            }
            else if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var perfil = await _perfilComercialService.GetPerfilByIdAsync(pedido.PerfilComercialId);
                if (perfil == null || perfil.EmpresaId != empresa.Id)
                {
                    return Forbid("No tienes permiso para actualizar este pedido");
                }
            }

            // Validar que no se puede cancelar un pedido entregado
            if (request.Estado == "Cancelado" && pedido.Estado == "Entregado")
            {
                return BadRequest(new { message = "No se puede cancelar un pedido que ya ha sido entregado" });
            }

            string? empresaId = null;
            if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                empresaId = empresa?.Id;
            }

            var pedidoActualizado = await _pedidoService.UpdatePedidoEstadoAsync(id, request.Estado, empresaId);
            return Ok(pedidoActualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}/repartidor")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Pedido>> UpdateRepartidorPosicion(string id, [FromBody] UpdateRepartidorRequest request)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            // Obtener el pedido para validar permisos
            var pedido = await _pedidoService.GetPedidoByIdAsync(id);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            // Validar permisos seg�n el rol
            if (rol == "Cliente")
            {
                // Los clientes solo pueden actualizar sus propios pedidos
                if (pedido.ClienteId != usuarioId)
                {
                    return Forbid("No tienes permiso para actualizar este pedido");
                }
            }
            else if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }

                var perfil = await _perfilComercialService.GetPerfilByIdAsync(pedido.PerfilComercialId);
                if (perfil == null || perfil.EmpresaId != empresa.Id)
                {
                    return Forbid("No tienes permiso para actualizar este pedido");
                }
            }

            var pedidoActualizado = await _pedidoService.UpdateRepartidorPosicionAsync(
                id,
                request.Latitud,
                request.Longitud,
                request.ProgresoRuta,
                request.RutaGeometry,
                request.DuracionRutaSegundos);

            return Ok(pedidoActualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}/puntuacion")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<Pedido>> UpdatePuntuacion(string id, [FromBody] UpdatePuntuacionRequest request)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            // Obtener el pedido para validar permisos
            var pedido = await _pedidoService.GetPedidoByIdAsync(id);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            // Validar permisos - solo el cliente puede puntuar su pedido
            if (rol == "Cliente")
            {
                if (pedido.ClienteId != usuarioId)
                {
                    return Forbid("No tienes permiso para puntuar este pedido");
                }
            }
            else
            {
                return Forbid("Solo los clientes pueden puntuar pedidos");
            }

            var pedidoActualizado = await _pedidoService.UpdatePuntuacionEntregaAsync(id, request.Puntuacion);
            return Ok(pedidoActualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

public class UpdateEstadoRequest
{
    public string Estado { get; set; } = string.Empty;
}

public class UpdatePuntuacionRequest
{
    public int Puntuacion { get; set; }
}

