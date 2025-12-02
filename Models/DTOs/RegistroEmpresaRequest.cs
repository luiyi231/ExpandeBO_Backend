namespace ExpandeBO_Backend.Models.DTOs;

public class RegistroEmpresaRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string NIT { get; set; } = string.Empty;
    public string? Direccion { get; set; }
}


