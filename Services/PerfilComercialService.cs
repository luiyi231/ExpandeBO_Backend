using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Services;

public class PerfilComercialService : IPerfilComercialService
{
    private readonly IPerfilComercialRepository _perfilComercialRepository;
    private readonly ISuscripcionEmpresaRepository _suscripcionEmpresaRepository;
    private readonly IPlanRepository _planRepository;

    public PerfilComercialService(
        IPerfilComercialRepository perfilComercialRepository,
        ISuscripcionEmpresaRepository suscripcionEmpresaRepository,
        IPlanRepository planRepository)
    {
        _perfilComercialRepository = perfilComercialRepository;
        _suscripcionEmpresaRepository = suscripcionEmpresaRepository;
        _planRepository = planRepository;
    }

    public async Task<PerfilComercial> CreatePerfilAsync(PerfilComercial perfil, string empresaId)
    {
        // Validar nombre del perfil
        if (string.IsNullOrWhiteSpace(perfil.Nombre))
        {
            throw new ArgumentException("El nombre del perfil comercial es requerido");
        }
        if (perfil.Nombre.Length > 50)
        {
            throw new ArgumentException("El nombre del perfil comercial no puede exceder 50 caracteres");
        }
        
        // Validar dirección
        if (!string.IsNullOrWhiteSpace(perfil.Direccion) && perfil.Direccion.Length > 50)
        {
            throw new ArgumentException("La dirección no puede exceder 50 caracteres");
        }
        
        // Validar que latitud y longitud sean obligatorios
        if (!perfil.Latitud.HasValue || !perfil.Longitud.HasValue)
        {
            throw new ArgumentException("La latitud y longitud son obligatorias para el perfil comercial");
        }

        // Validar límite de perfiles según el plan
        var suscripcion = await _suscripcionEmpresaRepository.GetActivaByEmpresaIdAsync(empresaId);
        if (suscripcion == null)
        {
            throw new InvalidOperationException("La empresa no tiene una suscripción activa");
        }

        var plan = await _planRepository.GetByIdAsync(suscripcion.PlanId);
        if (plan == null)
        {
            throw new InvalidOperationException("Plan no encontrado");
        }

        var perfilesExistentes = await _perfilComercialRepository.GetByEmpresaIdAsync(empresaId);
        if (perfilesExistentes.Count >= plan.MaxPerfilesComerciales)
        {
            throw new InvalidOperationException($"Has alcanzado el límite de perfiles comerciales permitidos por tu plan ({plan.MaxPerfilesComerciales})");
        }

        // Validar que no se repita la ciudad
        if (!string.IsNullOrEmpty(perfil.CiudadId))
        {
            var perfilConMismaCiudad = perfilesExistentes.FirstOrDefault(p => p.CiudadId == perfil.CiudadId);
            if (perfilConMismaCiudad != null)
            {
                throw new InvalidOperationException($"Ya tienes un perfil comercial en esta ciudad. No puedes tener múltiples perfiles en la misma ciudad.");
            }
        }

        perfil.EmpresaId = empresaId;
        perfil.FechaCreacion = DateTime.UtcNow;

        return await _perfilComercialRepository.CreateAsync(perfil);
    }

    public async Task<PerfilComercial> UpdatePerfilAsync(string perfilId, PerfilComercial perfil, string empresaId)
    {
        var perfilExistente = await _perfilComercialRepository.GetByIdAsync(perfilId);
        if (perfilExistente == null)
        {
            throw new KeyNotFoundException("Perfil comercial no encontrado");
        }

        if (perfilExistente.EmpresaId != empresaId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para actualizar este perfil comercial");
        }

        // Validar nombre del perfil
        if (string.IsNullOrWhiteSpace(perfil.Nombre))
        {
            throw new ArgumentException("El nombre del perfil comercial es requerido");
        }
        if (perfil.Nombre.Length > 50)
        {
            throw new ArgumentException("El nombre del perfil comercial no puede exceder 50 caracteres");
        }
        
        // Validar dirección
        if (!string.IsNullOrWhiteSpace(perfil.Direccion) && perfil.Direccion.Length > 50)
        {
            throw new ArgumentException("La dirección no puede exceder 50 caracteres");
        }
        
        // Validar que latitud y longitud sean obligatorios
        if (!perfil.Latitud.HasValue || !perfil.Longitud.HasValue)
        {
            throw new ArgumentException("La latitud y longitud son obligatorias para el perfil comercial");
        }

        // Validar que no se repita la ciudad (excepto si es el mismo perfil)
        if (!string.IsNullOrEmpty(perfil.CiudadId))
        {
            var perfilesExistentes = await _perfilComercialRepository.GetByEmpresaIdAsync(empresaId);
            var perfilConMismaCiudad = perfilesExistentes.FirstOrDefault(p => p.CiudadId == perfil.CiudadId && p.Id != perfilId);
            if (perfilConMismaCiudad != null)
            {
                throw new InvalidOperationException($"Ya tienes un perfil comercial en esta ciudad. No puedes tener múltiples perfiles en la misma ciudad.");
            }
        }

        perfil.Id = perfilId;
        perfil.EmpresaId = empresaId;
        perfil.FechaUltimaActualizacion = DateTime.UtcNow;

        return await _perfilComercialRepository.UpdateAsync(perfil);
    }

    public async Task<bool> DeletePerfilAsync(string perfilId, string empresaId)
    {
        var perfil = await _perfilComercialRepository.GetByIdAsync(perfilId);
        if (perfil == null)
        {
            return false;
        }

        if (perfil.EmpresaId != empresaId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para eliminar este perfil comercial");
        }

        return await _perfilComercialRepository.DeleteAsync(perfilId);
    }

    public async Task<PerfilComercial?> GetPerfilByIdAsync(string id)
    {
        return await _perfilComercialRepository.GetByIdAsync(id);
    }

    public async Task<List<PerfilComercial>> GetPerfilesByEmpresaAsync(string empresaId)
    {
        return await _perfilComercialRepository.GetByEmpresaIdAsync(empresaId);
    }

    public async Task<List<PerfilComercial>> GetPerfilesActivosAsync()
    {
        return await _perfilComercialRepository.GetActivosAsync();
    }
}


