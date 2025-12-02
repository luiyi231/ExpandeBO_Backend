namespace ExpandeBO_Backend.Models.DTOs;

public class RegistroRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Rol { get; set; } = "Cliente"; // Cliente, Empresa, Administrador
}


