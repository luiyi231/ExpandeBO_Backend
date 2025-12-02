using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanesController : ControllerBase
{
    private readonly IPlanRepository _planRepository;

    public PlanesController(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Plan>>> GetPlanes()
    {
        try
        {
            var planes = await _planRepository.GetActivosAsync();
            return Ok(planes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Plan>> GetPlan(string id)
    {
        try
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan == null)
            {
                return NotFound(new { message = "Plan no encontrado" });
            }

            return Ok(plan);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Plan>> CreatePlan([FromBody] Plan plan)
    {
        try
        {
            plan.FechaCreacion = DateTime.UtcNow;
            var nuevoPlan = await _planRepository.CreateAsync(plan);
            return CreatedAtAction(nameof(GetPlan), new { id = nuevoPlan.Id }, nuevoPlan);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Plan>> UpdatePlan(string id, [FromBody] Plan plan)
    {
        try
        {
            var planExistente = await _planRepository.GetByIdAsync(id);
            if (planExistente == null)
            {
                return NotFound(new { message = "Plan no encontrado" });
            }

            plan.Id = id;
            var planActualizado = await _planRepository.UpdateAsync(plan);
            return Ok(planActualizado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}


