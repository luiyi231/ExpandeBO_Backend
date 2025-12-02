using ExpandeBO_Backend.Models.DTOs;

namespace ExpandeBO_Backend.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegistroAsync(RegistroRequest request);
    Task<AuthResponse> RegistroEmpresaAsync(RegistroEmpresaRequest request);
    string GenerateJwtToken(string userId, string email, string rol);
}


