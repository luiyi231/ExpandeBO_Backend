using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpresasController : ControllerBase
{
    private readonly IEmpresaRepository _empresaRepository;

    public EmpresasController(IEmpresaRepository empresaRepository)
    {
        _empresaRepository = empresaRepository;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<Empresa>>> GetEmpresas()
    {
        try
        {
            var empresas = await _empresaRepository.GetAllAsync();
            return Ok(empresas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Empresa>> GetEmpresa(string id)
    {
        try
        {
            var empresa = await _empresaRepository.GetByIdAsync(id);
            if (empresa == null)
            {
                return NotFound(new { message = "Empresa no encontrada" });
            }

            return Ok(empresa);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}/activar")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Empresa>> ActivarEmpresa(string id)
    {
        try
        {
            var empresa = await _empresaRepository.GetByIdAsync(id);
            if (empresa == null)
            {
                return NotFound(new { message = "Empresa no encontrada" });
            }

            empresa.Activa = true;
            empresa.FechaUltimaActualizacion = DateTime.UtcNow;
            var empresaActualizada = await _empresaRepository.UpdateAsync(empresa);
            return Ok(empresaActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}/suspender")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Empresa>> SuspenderEmpresa(string id)
    {
        try
        {
            var empresa = await _empresaRepository.GetByIdAsync(id);
            if (empresa == null)
            {
                return NotFound(new { message = "Empresa no encontrada" });
            }

            empresa.Activa = false;
            empresa.FechaUltimaActualizacion = DateTime.UtcNow;
            var empresaActualizada = await _empresaRepository.UpdateAsync(empresa);
            return Ok(empresaActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteEmpresa(string id)
    {
        try
        {
            var eliminada = await _empresaRepository.DeleteAsync(id);
            if (!eliminada)
            {
                return NotFound(new { message = "Empresa no encontrada" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

