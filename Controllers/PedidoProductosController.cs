using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidoProductosController : ControllerBase
{
    private readonly IPedidoProductoRepository _pedidoProductoRepository;

    public PedidoProductosController(IPedidoProductoRepository pedidoProductoRepository)
    {
        _pedidoProductoRepository = pedidoProductoRepository;
    }

    [HttpGet]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<List<PedidoProducto>>> GetPedidoProductos()
    {
        try
        {
            var pedidoProductos = await _pedidoProductoRepository.GetAllAsync();
            return Ok(pedidoProductos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("pedido/{pedidoId}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<List<PedidoProducto>>> GetByPedidoId(string pedidoId)
    {
        try
        {
            var pedidoProductos = await _pedidoProductoRepository.GetByPedidoIdAsync(pedidoId);
            return Ok(pedidoProductos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("producto/{productoId}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<List<PedidoProducto>>> GetByProductoId(string productoId)
    {
        try
        {
            var pedidoProductos = await _pedidoProductoRepository.GetByProductoIdAsync(productoId);
            return Ok(pedidoProductos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<PedidoProducto>> GetPedidoProducto(string id)
    {
        try
        {
            var pedidoProducto = await _pedidoProductoRepository.GetByIdAsync(id);
            if (pedidoProducto == null)
            {
                return NotFound(new { message = "Relación pedido-producto no encontrada" });
            }

            return Ok(pedidoProducto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<PedidoProducto>> CreatePedidoProducto([FromBody] PedidoProducto pedidoProducto)
    {
        try
        {
            pedidoProducto.FechaCreacion = DateTime.UtcNow;
            var nuevoPedidoProducto = await _pedidoProductoRepository.CreateAsync(pedidoProducto);
            return CreatedAtAction(nameof(GetPedidoProducto), new { id = nuevoPedidoProducto.Id }, nuevoPedidoProducto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<ActionResult<PedidoProducto>> UpdatePedidoProducto(string id, [FromBody] PedidoProducto pedidoProducto)
    {
        try
        {
            var pedidoProductoExistente = await _pedidoProductoRepository.GetByIdAsync(id);
            if (pedidoProductoExistente == null)
            {
                return NotFound(new { message = "Relación pedido-producto no encontrada" });
            }

            pedidoProducto.Id = id;
            var pedidoProductoActualizado = await _pedidoProductoRepository.UpdateAsync(pedidoProducto);
            return Ok(pedidoProductoActualizado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ClienteOrEmpresaOrAdmin")]
    public async Task<IActionResult> DeletePedidoProducto(string id)
    {
        try
        {
            var eliminado = await _pedidoProductoRepository.DeleteAsync(id);
            if (!eliminado)
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

