using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriasController(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Categoria>>> GetCategorias()
    {
        try
        {
            var categorias = await _categoriaRepository.GetActivasAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Categoria>> GetCategoria(string id)
    {
        try
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            return Ok(categoria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Categoria>> CreateCategoria([FromBody] Categoria categoria)
    {
        try
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                return BadRequest(new { message = "El nombre de la categoría es requerido" });
            }
            if (categoria.Nombre.Length > 100)
            {
                return BadRequest(new { message = "El nombre de la categoría no puede exceder 100 caracteres" });
            }

            categoria.FechaCreacion = DateTime.UtcNow;
            var nuevaCategoria = await _categoriaRepository.CreateAsync(categoria);
            return CreatedAtAction(nameof(GetCategoria), new { id = nuevaCategoria.Id }, nuevaCategoria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Categoria>> UpdateCategoria(string id, [FromBody] Categoria categoria)
    {
        try
        {
            var categoriaExistente = await _categoriaRepository.GetByIdAsync(id);
            if (categoriaExistente == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            // Validar nombre
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                return BadRequest(new { message = "El nombre de la categoría es requerido" });
            }
            if (categoria.Nombre.Length > 100)
            {
                return BadRequest(new { message = "El nombre de la categoría no puede exceder 100 caracteres" });
            }

            categoria.Id = id;
            var categoriaActualizada = await _categoriaRepository.UpdateAsync(categoria);
            return Ok(categoriaActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteCategoria(string id)
    {
        try
        {
            var eliminada = await _categoriaRepository.DeleteAsync(id);
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


