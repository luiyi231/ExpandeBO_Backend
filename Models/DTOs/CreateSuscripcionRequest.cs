namespace ExpandeBO_Backend.Models.DTOs;

public class CreateSuscripcionRequest
{
    public string PlanId { get; set; } = string.Empty;
    public string? EmpresaId { get; set; } // Solo para administradores
}

