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
    private readonly IClienteService _clienteService;

    public ProductosController(
        IProductoService productoService,
        IEmpresaRepository empresaRepository,
        IPerfilComercialService perfilComercialService,
        IClienteService clienteService)
    {
        _productoService = productoService;
        _empresaRepository = empresaRepository;
        _perfilComercialService = perfilComercialService;
        _clienteService = clienteService;
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
            // Si el usuario está autenticado y es Cliente, filtrar por su ciudad
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            List<Producto> productos;

            if (!string.IsNullOrEmpty(usuarioId) && rol == "Cliente")
            {
                var cliente = await _clienteService.GetClienteByUsuarioIdAsync(usuarioId);
                if (cliente != null && !string.IsNullOrEmpty(cliente.CiudadId))
                {
                    // Obtener productos de la ciudad del cliente
                    var productosPorCiudad = await _productoService.GetProductosByCiudadAsync(cliente.CiudadId);
                    // Filtrar por categoría
                    productos = productosPorCiudad.Where(p => p.CategoriaId == categoriaId).ToList();
                    return Ok(productos);
                }
            }

            // Si no es cliente o no tiene ciudad asignada, devolver todos los productos de la categoría
            productos = await _productoService.GetProductosByCategoriaAsync(categoriaId);
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
            // Si el usuario está autenticado y es Cliente, filtrar por su ciudad
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rol = User.FindFirst("Rol")?.Value;

            if (!string.IsNullOrEmpty(usuarioId) && rol == "Cliente")
            {
                var cliente = await _clienteService.GetClienteByUsuarioIdAsync(usuarioId);
                if (cliente != null && !string.IsNullOrEmpty(cliente.CiudadId))
                {
                    // Filtrar productos por ciudad del cliente
                    var productos = await _productoService.GetProductosByCiudadAsync(cliente.CiudadId);
                    return Ok(productos);
                }
            }

            // Si no es cliente o no tiene ciudad asignada, devolver todos los productos disponibles
            var todosProductos = await _productoService.GetProductosDisponiblesAsync();
            return Ok(todosProductos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("todos")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<List<object>>> GetTodosLosProductos()
    {
        try
        {
            var productos = await _productoService.GetTodosLosProductosAsync();
            var resultado = new List<object>();

            foreach (var producto in productos)
            {
                var perfil = await _perfilComercialService.GetPerfilByIdAsync(producto.PerfilComercialId);
                if (perfil == null) continue;

                var empresa = await _empresaRepository.GetByIdAsync(perfil.EmpresaId);
                if (empresa == null) continue;

                resultado.Add(new
                {
                    id = producto.Id,
                    nombre = producto.Nombre,
                    descripcion = producto.Descripcion,
                    precio = producto.Precio,
                    stock = producto.Stock,
                    disponible = producto.Disponible,
                    imagenUrl = producto.ImagenUrl,
                    categoriaId = producto.CategoriaId,
                    subcategoriaId = producto.SubcategoriaId,
                    perfilComercialId = producto.PerfilComercialId,
                    fechaCreacion = producto.FechaCreacion,
                    fechaUltimaActualizacion = producto.FechaUltimaActualizacion,
                    empresa = new
                    {
                        id = empresa.Id,
                        razonSocial = empresa.RazonSocial,
                        nit = empresa.NIT,
                        email = empresa.Email
                    },
                    perfilComercial = new
                    {
                        id = perfil.Id,
                        nombre = perfil.Nombre,
                        direccion = perfil.Direccion,
                        ciudadId = perfil.CiudadId
                    }
                });
            }

            return Ok(resultado);
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

