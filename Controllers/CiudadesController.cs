using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CiudadesController : ControllerBase
{
    private readonly ICiudadRepository _ciudadRepository;

    public CiudadesController(ICiudadRepository ciudadRepository)
    {
        _ciudadRepository = ciudadRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Ciudad>>> GetCiudades()
    {
        try
        {
            var ciudades = await _ciudadRepository.GetActivasAsync();
            return Ok(ciudades);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Ciudad>> GetCiudad(string id)
    {
        try
        {
            var ciudad = await _ciudadRepository.GetByIdAsync(id);
            if (ciudad == null)
            {
                return NotFound(new { message = "Ciudad no encontrada" });
            }

            return Ok(ciudad);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Ciudad>> CreateCiudad([FromBody] Ciudad ciudad)
    {
        try
        {
            ciudad.FechaCreacion = DateTime.UtcNow;
            var nuevaCiudad = await _ciudadRepository.CreateAsync(ciudad);
            return CreatedAtAction(nameof(GetCiudad), new { id = nuevaCiudad.Id }, nuevaCiudad);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Ciudad>> UpdateCiudad(string id, [FromBody] Ciudad ciudad)
    {
        try
        {
            var ciudadExistente = await _ciudadRepository.GetByIdAsync(id);
            if (ciudadExistente == null)
            {
                return NotFound(new { message = "Ciudad no encontrada" });
            }

            ciudad.Id = id;
            var ciudadActualizada = await _ciudadRepository.UpdateAsync(ciudad);
            return Ok(ciudadActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}


