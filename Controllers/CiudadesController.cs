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
    private readonly IClienteRepository _clienteRepository;
    private readonly IPerfilComercialRepository _perfilComercialRepository;

    public CiudadesController(
        ICiudadRepository ciudadRepository,
        IClienteRepository clienteRepository,
        IPerfilComercialRepository perfilComercialRepository)
    {
        _ciudadRepository = ciudadRepository;
        _clienteRepository = clienteRepository;
        _perfilComercialRepository = perfilComercialRepository;
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

    [HttpGet("{id}/relaciones")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CiudadRelacionesResponse>> GetCiudadRelaciones(string id)
    {
        try
        {
            var ciudad = await _ciudadRepository.GetByIdAsync(id);
            if (ciudad == null)
            {
                return NotFound(new { message = "Ciudad no encontrada" });
            }

            var clientes = await _clienteRepository.GetByCiudadIdAsync(id);
            var perfilesComerciales = await _perfilComercialRepository.GetByCiudadIdAsync(id);

            return Ok(new CiudadRelacionesResponse
            {
                CiudadId = id,
                CiudadNombre = ciudad.Nombre,
                Clientes = clientes,
                PerfilesComerciales = perfilesComerciales,
                TieneRelaciones = clientes.Count > 0 || perfilesComerciales.Count > 0
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteCiudad(string id, [FromQuery] bool eliminarRelaciones = false)
    {
        try
        {
            var ciudad = await _ciudadRepository.GetByIdAsync(id);
            if (ciudad == null)
            {
                return NotFound(new { message = "Ciudad no encontrada" });
            }

            // Verificar relaciones
            var clientes = await _clienteRepository.GetByCiudadIdAsync(id);
            var perfilesComerciales = await _perfilComercialRepository.GetByCiudadIdAsync(id);

            if (clientes.Count > 0 || perfilesComerciales.Count > 0)
            {
                if (!eliminarRelaciones)
                {
                    return BadRequest(new
                    {
                        message = "La ciudad tiene relaciones asociadas. Use el parámetro eliminarRelaciones=true para eliminarlas también.",
                        relaciones = new
                        {
                            clientes = clientes.Count,
                            perfilesComerciales = perfilesComerciales.Count
                        }
                    });
                }

                // Eliminar relaciones
                foreach (var cliente in clientes)
                {
                    await _clienteRepository.DeleteAsync(cliente.Id!);
                }

                foreach (var perfil in perfilesComerciales)
                {
                    await _perfilComercialRepository.DeleteAsync(perfil.Id!);
                }
            }

            // Eliminar la ciudad
            var eliminado = await _ciudadRepository.DeleteAsync(id);
            if (!eliminado)
            {
                return NotFound(new { message = "No se pudo eliminar la ciudad" });
            }

            return Ok(new { message = "Ciudad eliminada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("todas")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<Ciudad>>> GetAllCiudades()
    {
        try
        {
            var ciudades = await _ciudadRepository.GetAllAsync();
            return Ok(ciudades);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

public class CiudadRelacionesResponse
{
    public string CiudadId { get; set; } = string.Empty;
    public string CiudadNombre { get; set; } = string.Empty;
    public List<Cliente> Clientes { get; set; } = new();
    public List<PerfilComercial> PerfilesComerciales { get; set; } = new();
    public bool TieneRelaciones { get; set; }
}


