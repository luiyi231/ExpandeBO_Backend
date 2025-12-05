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
public class PerfilesController : ControllerBase
{
    private readonly IPerfilComercialService _perfilComercialService;
    private readonly IEmpresaRepository _empresaRepository;

    public PerfilesController(
        IPerfilComercialService perfilComercialService,
        IEmpresaRepository empresaRepository)
    {
        _perfilComercialService = perfilComercialService;
        _empresaRepository = empresaRepository;
    }

    [HttpGet("mis-perfiles")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<List<PerfilComercial>>> GetMisPerfiles()
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var rol = User.FindFirst("Rol")?.Value;
            if (rol == "Administrador")
            {
                var perfiles = await _perfilComercialService.GetPerfilesActivosAsync();
                return Ok(perfiles);
            }

            var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId);
            if (empresa == null)
            {
                return NotFound(new { message = "Empresa no encontrada" });
            }

            var misPerfiles = await _perfilComercialService.GetPerfilesByEmpresaAsync(empresa.Id!);
            return Ok(misPerfiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<PerfilComercial>> GetPerfil(string id)
    {
        try
        {
            var perfil = await _perfilComercialService.GetPerfilByIdAsync(id);
            if (perfil == null)
            {
                return NotFound(new { message = "Perfil comercial no encontrado" });
            }

            // Validar que las empresas solo vean sus propios perfiles
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol == "Empresa")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null || empresa.Id != perfil.EmpresaId)
                {
                    return Forbid("No tienes permiso para ver este perfil comercial");
                }
            }

            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<PerfilComercial>> CreatePerfil([FromBody] PerfilComercial perfil)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var rol = User.FindFirst("Rol")?.Value;
            string empresaId;

            if (rol == "Administrador")
            {
                if (string.IsNullOrEmpty(perfil.EmpresaId))
                {
                    return BadRequest(new { message = "EmpresaId es requerido para administradores" });
                }
                empresaId = perfil.EmpresaId;
            }
            else
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }
                empresaId = empresa.Id!;
            }

            var nuevoPerfil = await _perfilComercialService.CreatePerfilAsync(perfil, empresaId);
            return CreatedAtAction(nameof(GetPerfil), new { id = nuevoPerfil.Id }, nuevoPerfil);
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

    [HttpPut("{id}")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<PerfilComercial>> UpdatePerfil(string id, [FromBody] PerfilComercial perfil)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            var rol = User.FindFirst("Rol")?.Value;
            string empresaId;

            if (rol == "Administrador")
            {
                var perfilExistente = await _perfilComercialService.GetPerfilByIdAsync(id);
                if (perfilExistente == null)
                {
                    return NotFound();
                }
                empresaId = perfilExistente.EmpresaId;
            }
            else
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId);
                if (empresa == null)
                {
                    return NotFound(new { message = "Empresa no encontrada" });
                }
                empresaId = empresa.Id!;
            }

            var perfilActualizado = await _perfilComercialService.UpdatePerfilAsync(id, perfil, empresaId);
            return Ok(perfilActualizado);
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

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PerfilComercial>>> GetPerfilesActivos()
    {
        try
        {
            var perfiles = await _perfilComercialService.GetPerfilesActivosAsync();
            return Ok(perfiles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

