using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Models.DTOs;
using ExpandeBO_Backend.Repositories;
using System.Security.Claims;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuscripcionesController : ControllerBase
{
    private readonly ISuscripcionEmpresaRepository _suscripcionRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPlanRepository _planRepository;

    public SuscripcionesController(
        ISuscripcionEmpresaRepository suscripcionRepository,
        IEmpresaRepository empresaRepository,
        IPlanRepository planRepository)
    {
        _suscripcionRepository = suscripcionRepository;
        _empresaRepository = empresaRepository;
        _planRepository = planRepository;
    }

    [HttpGet("mi-suscripcion")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<SuscripcionEmpresa>> GetMiSuscripcion()
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
                // Para admin, se puede pasar empresaId como query param
                var empresaIdParam = Request.Query["empresaId"].FirstOrDefault();
                if (string.IsNullOrEmpty(empresaIdParam))
                {
                    return BadRequest(new { message = "empresaId es requerido para administradores" });
                }
                empresaId = empresaIdParam;
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

            var suscripcion = await _suscripcionRepository.GetActivaByEmpresaIdAsync(empresaId);
            if (suscripcion == null)
            {
                return NotFound(new { message = "No se encontró una suscripción activa" });
            }

            return Ok(suscripcion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("mis-suscripciones")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<List<SuscripcionEmpresa>>> GetMisSuscripciones()
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
                var empresaIdParam = Request.Query["empresaId"].FirstOrDefault();
                if (string.IsNullOrEmpty(empresaIdParam))
                {
                    return BadRequest(new { message = "empresaId es requerido para administradores" });
                }
                empresaId = empresaIdParam;
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

            var suscripciones = await _suscripcionRepository.GetByEmpresaIdAsync(empresaId);
            return Ok(suscripciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<SuscripcionEmpresa>> GetSuscripcion(string id)
    {
        try
        {
            var suscripcion = await _suscripcionRepository.GetByIdAsync(id);
            if (suscripcion == null)
            {
                return NotFound(new { message = "Suscripción no encontrada" });
            }

            // Verificar que el usuario tenga acceso a esta suscripción
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol != "Administrador")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null || empresa.Id != suscripcion.EmpresaId)
                {
                    return Forbid();
                }
            }

            return Ok(suscripcion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<SuscripcionEmpresa>> CreateSuscripcion([FromBody] CreateSuscripcionRequest request)
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
                if (string.IsNullOrEmpty(request.EmpresaId))
                {
                    return BadRequest(new { message = "EmpresaId es requerido para administradores" });
                }
                empresaId = request.EmpresaId;
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

            // Verificar que el plan existe
            var plan = await _planRepository.GetByIdAsync(request.PlanId);
            if (plan == null)
            {
                return NotFound(new { message = "Plan no encontrado" });
            }

            if (!plan.Activo)
            {
                return BadRequest(new { message = "El plan seleccionado no está activo" });
            }

            // Desactivar suscripciones anteriores de la empresa
            var suscripcionesAnteriores = await _suscripcionRepository.GetByEmpresaIdAsync(empresaId);
            foreach (var suscripcion in suscripcionesAnteriores.Where(s => s.Activa))
            {
                suscripcion.Activa = false;
                await _suscripcionRepository.UpdateAsync(suscripcion);
            }

            // Crear nueva suscripción
            var nuevaSuscripcion = new SuscripcionEmpresa
            {
                EmpresaId = empresaId,
                PlanId = request.PlanId,
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddDays(plan.DuracionDias),
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            };

            var suscripcionCreada = await _suscripcionRepository.CreateAsync(nuevaSuscripcion);
            return CreatedAtAction(nameof(GetSuscripcion), new { id = suscripcionCreada.Id }, suscripcionCreada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}/renovar")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<SuscripcionEmpresa>> RenovarSuscripcion(string id)
    {
        try
        {
            var suscripcion = await _suscripcionRepository.GetByIdAsync(id);
            if (suscripcion == null)
            {
                return NotFound(new { message = "Suscripción no encontrada" });
            }

            // Verificar permisos
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol != "Administrador")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null || empresa.Id != suscripcion.EmpresaId)
                {
                    return Forbid();
                }
            }

            // Obtener el plan para calcular nueva fecha de fin
            var plan = await _planRepository.GetByIdAsync(suscripcion.PlanId);
            if (plan == null)
            {
                return NotFound(new { message = "Plan asociado no encontrado" });
            }

            // Renovar desde la fecha actual o desde fechaFin si ya expiró
            var fechaBase = suscripcion.FechaFin > DateTime.UtcNow ? suscripcion.FechaFin : DateTime.UtcNow;
            suscripcion.FechaFin = fechaBase.AddDays(plan.DuracionDias);
            suscripcion.Activa = true;

            var suscripcionActualizada = await _suscripcionRepository.UpdateAsync(suscripcion);
            return Ok(suscripcionActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}/cancelar")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<SuscripcionEmpresa>> CancelarSuscripcion(string id)
    {
        try
        {
            var suscripcion = await _suscripcionRepository.GetByIdAsync(id);
            if (suscripcion == null)
            {
                return NotFound(new { message = "Suscripción no encontrada" });
            }

            // Verificar permisos
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (rol != "Administrador")
            {
                var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioId!);
                if (empresa == null || empresa.Id != suscripcion.EmpresaId)
                {
                    return Forbid();
                }
            }

            suscripcion.Activa = false;
            var suscripcionActualizada = await _suscripcionRepository.UpdateAsync(suscripcion);
            return Ok(suscripcionActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteSuscripcion(string id)
    {
        try
        {
            var suscripcion = await _suscripcionRepository.GetByIdAsync(id);
            if (suscripcion == null)
            {
                return NotFound(new { message = "Suscripción no encontrada" });
            }

            var eliminado = await _suscripcionRepository.DeleteAsync(id);
            if (!eliminado)
            {
                return StatusCode(500, new { message = "Error al eliminar la suscripción" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

