using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Models.DTOs;
using ExpandeBO_Backend.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace ExpandeBO_Backend.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IEmpresaRepository empresaRepository,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _empresaRepository = empresaRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

        if (usuario == null || !usuario.Activo)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        string? empresaId = null;
        if (usuario.Rol == "Empresa")
        {
            var empresa = await _empresaRepository.GetByUsuarioIdAsync(usuario.Id!);
            empresaId = empresa?.Id;
        }

        var token = GenerateJwtToken(usuario.Id!, usuario.Email, usuario.Rol);

        return new AuthResponse
        {
            Token = token,
            UsuarioId = usuario.Id!,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol,
            EmpresaId = empresaId
        };
    }

    public async Task<AuthResponse> RegistroAsync(RegistroRequest request)
    {
        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (usuarioExistente != null)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var usuario = new Usuario
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Telefono = request.Telefono,
            Rol = request.Rol,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        usuario = await _usuarioRepository.CreateAsync(usuario);

        var token = GenerateJwtToken(usuario.Id!, usuario.Email, usuario.Rol);

        return new AuthResponse
        {
            Token = token,
            UsuarioId = usuario.Id!,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol
        };
    }

    public async Task<AuthResponse> RegistroEmpresaAsync(RegistroEmpresaRequest request)
    {
        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (usuarioExistente != null)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var usuario = new Usuario
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Telefono = request.Telefono,
            Rol = "Empresa",
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        usuario = await _usuarioRepository.CreateAsync(usuario);

        var empresa = new Empresa
        {
            UsuarioId = usuario.Id!,
            RazonSocial = request.RazonSocial,
            NIT = request.NIT,
            Direccion = request.Direccion,
            Telefono = request.Telefono,
            Email = request.Email,
            Activa = true,
            FechaCreacion = DateTime.UtcNow
        };

        empresa = await _empresaRepository.CreateAsync(empresa);

        var token = GenerateJwtToken(usuario.Id!, usuario.Email, usuario.Rol);

        return new AuthResponse
        {
            Token = token,
            UsuarioId = usuario.Id!,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol,
            EmpresaId = empresa.Id
        };
    }

    public string GenerateJwtToken(string userId, string email, string rol)
    {
        var jwtSecretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ExpandeBO";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "ExpandeBOUsers";
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "1440");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim("Rol", rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


