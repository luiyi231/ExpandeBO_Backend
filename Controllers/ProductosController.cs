using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Services;
using ExpandeBO_Backend.Repositories;
using System.Security.Claims;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPerfilComercialService _perfilComercialService;

    public ProductosController(
        IProductoService productoService,
        IEmpresaRepository empresaRepository,
        IPerfilComercialService perfilComercialService)
    {
        _productoService = productoService;
        _empresaRepository = empresaRepository;
        _perfilComercialService = perfilComercialService;
    }

    [HttpGet("perfil/{idPerfil}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Producto>>> GetProductosByPerfil(string idPerfil)
    {
        try
        {
            var productos = await _productoService.GetProductosByPerfilAsync(idPerfil);
            return Ok(productos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("categoria/{categoriaId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Producto>>> GetProductosByCategoria(string categoriaId)
    {
        try
        {
            var productos = await _productoService.GetProductosByCategoriaAsync(categoriaId);
            return Ok(productos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("disponibles")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Producto>>> GetProductosDisponibles()
    {
        try
        {
            var productos = await _productoService.GetProductosDisponiblesAsync();
            return Ok(productos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Producto>> GetProducto(string id)
    {
        try
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            return Ok(producto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<Producto>> CreateProducto([FromBody] Producto producto)
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
                // Para admin, necesitamos obtener la empresa del perfil comercial
                var perfil = await _perfilComercialService.GetPerfilByIdAsync(producto.PerfilComercialId);
                if (perfil == null)
                {
                    return BadRequest(new { message = "Perfil comercial no encontrado" });
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

            var nuevoProducto = await _productoService.CreateProductoAsync(producto, empresaId);
            return CreatedAtAction(nameof(GetProducto), new { id = nuevoProducto.Id }, nuevoProducto);
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

    [HttpPut("{id}")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<ActionResult<Producto>> UpdateProducto(string id, [FromBody] Producto producto)
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
                var productoExistente = await _productoService.GetProductoByIdAsync(id);
                if (productoExistente == null)
                {
                    return NotFound();
                }
                var perfil = await _perfilComercialService.GetPerfilByIdAsync(productoExistente.PerfilComercialId);
                if (perfil == null)
                {
                    return BadRequest(new { message = "Perfil comercial no encontrado" });
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

            var productoActualizado = await _productoService.UpdateProductoAsync(id, producto, empresaId);
            return Ok(productoActualizado);
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

    [HttpDelete("{id}")]
    [Authorize(Policy = "EmpresaOrAdmin")]
    public async Task<IActionResult> DeleteProducto(string id)
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
                var productoExistente = await _productoService.GetProductoByIdAsync(id);
                if (productoExistente == null)
                {
                    return NotFound();
                }
                var perfil = await _perfilComercialService.GetPerfilByIdAsync(productoExistente.PerfilComercialId);
                if (perfil == null)
                {
                    return BadRequest(new { message = "Perfil comercial no encontrado" });
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

            var eliminado = await _productoService.DeleteProductoAsync(id, empresaId);
            if (!eliminado)
            {
                return NotFound();
            }

            return NoContent();
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
}

