namespace ExpandeBO_Backend.Models.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? EmpresaId { get; set; }
}


