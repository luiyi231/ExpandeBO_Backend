using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubcategoriasController : ControllerBase
{
    private readonly ISubcategoriaRepository _subcategoriaRepository;

    public SubcategoriasController(ISubcategoriaRepository subcategoriaRepository)
    {
        _subcategoriaRepository = subcategoriaRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Subcategoria>>> GetSubcategorias([FromQuery] string? categoriaId = null)
    {
        try
        {
            List<Subcategoria> subcategorias;
            if (!string.IsNullOrEmpty(categoriaId))
            {
                subcategorias = await _subcategoriaRepository.GetByCategoriaIdAsync(categoriaId);
            }
            else
            {
                subcategorias = await _subcategoriaRepository.GetActivasAsync();
            }

            return Ok(subcategorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Subcategoria>> GetSubcategoria(string id)
    {
        try
        {
            var subcategoria = await _subcategoriaRepository.GetByIdAsync(id);
            if (subcategoria == null)
            {
                return NotFound(new { message = "Subcategoría no encontrada" });
            }

            return Ok(subcategoria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Subcategoria>> CreateSubcategoria([FromBody] Subcategoria subcategoria)
    {
        try
        {
            subcategoria.FechaCreacion = DateTime.UtcNow;
            var nuevaSubcategoria = await _subcategoriaRepository.CreateAsync(subcategoria);
            return CreatedAtAction(nameof(GetSubcategoria), new { id = nuevaSubcategoria.Id }, nuevaSubcategoria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Subcategoria>> UpdateSubcategoria(string id, [FromBody] Subcategoria subcategoria)
    {
        try
        {
            var subcategoriaExistente = await _subcategoriaRepository.GetByIdAsync(id);
            if (subcategoriaExistente == null)
            {
                return NotFound(new { message = "Subcategoría no encontrada" });
            }

            subcategoria.Id = id;
            var subcategoriaActualizada = await _subcategoriaRepository.UpdateAsync(subcategoria);
            return Ok(subcategoriaActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteSubcategoria(string id)
    {
        try
        {
            var eliminada = await _subcategoriaRepository.DeleteAsync(id);
            if (!eliminada)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}


