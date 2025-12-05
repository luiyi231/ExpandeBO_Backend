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
    private readonly IClienteService _clienteService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IEmpresaRepository empresaRepository,
        IClienteService clienteService,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _empresaRepository = empresaRepository;
        _clienteService = clienteService;
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
        // Validar email
        if (!ValidarEmail(request.Email))
        {
            throw new ArgumentException("Email inválido. Use solo letras, números y los símbolos . - _ antes del @");
        }

        // Validar nombre y apellido
        if (!ValidarNombreApellido(request.Nombre))
        {
            throw new ArgumentException("El nombre solo puede contener letras y espacios, máximo 50 caracteres");
        }
        if (!ValidarNombreApellido(request.Apellido))
        {
            throw new ArgumentException("El apellido solo puede contener letras y espacios, máximo 50 caracteres");
        }
        
        // Validar teléfono si se proporciona (8 dígitos)
        if (!string.IsNullOrWhiteSpace(request.Telefono))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Telefono, @"^\d{8}$"))
            {
                throw new ArgumentException("El teléfono debe tener exactamente 8 dígitos");
            }
        }
        
        // Validar dirección si se proporciona (máximo 50 caracteres)
        if (!string.IsNullOrWhiteSpace(request.Direccion) && request.Direccion.Length > 50)
        {
            throw new ArgumentException("La dirección no puede exceder 50 caracteres");
        }

        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (usuarioExistente != null)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // Validar que si es Cliente, tenga CiudadId
        if (request.Rol == "Cliente" && string.IsNullOrEmpty(request.CiudadId))
        {
            throw new InvalidOperationException("La ciudad es requerida para clientes");
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

        // Si es Cliente, crear el perfil de cliente
        if (request.Rol == "Cliente" && !string.IsNullOrEmpty(request.CiudadId))
        {
            try
            {
                await _clienteService.CreateClienteAsync(usuario.Id!, request.CiudadId, request.Direccion);
            }
            catch (Exception ex)
            {
                // Si falla la creación del cliente, eliminar el usuario creado
                await _usuarioRepository.DeleteAsync(usuario.Id!);
                throw new InvalidOperationException($"Error al crear perfil de cliente: {ex.Message}");
            }
        }

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
        // Validar email
        if (!ValidarEmail(request.Email))
        {
            throw new ArgumentException("Email inválido. Use solo letras, números y los símbolos . - _ antes del @");
        }

        // Validar nombre y apellido
        if (!ValidarNombreApellido(request.Nombre))
        {
            throw new ArgumentException("El nombre solo puede contener letras y espacios, máximo 50 caracteres");
        }
        if (!ValidarNombreApellido(request.Apellido))
        {
            throw new ArgumentException("El apellido solo puede contener letras y espacios, máximo 50 caracteres");
        }
        
        // Validar teléfono si se proporciona (8 dígitos)
        if (!string.IsNullOrWhiteSpace(request.Telefono))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Telefono, @"^\d{8}$"))
            {
                throw new ArgumentException("El teléfono debe tener exactamente 8 dígitos");
            }
        }
        
        // Validar dirección si se proporciona (máximo 50 caracteres)
        if (!string.IsNullOrWhiteSpace(request.Direccion) && request.Direccion.Length > 50)
        {
            throw new ArgumentException("La dirección no puede exceder 50 caracteres");
        }

        // Validar razón social
        if (string.IsNullOrWhiteSpace(request.RazonSocial))
        {
            throw new ArgumentException("La razón social es requerida");
        }
        if (request.RazonSocial.Length > 50)
        {
            throw new ArgumentException("La razón social no puede exceder 50 caracteres");
        }
        
        // Validar NIT (7-10 dígitos)
        if (string.IsNullOrWhiteSpace(request.NIT))
        {
            throw new ArgumentException("El NIT es requerido");
        }
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.NIT, @"^\d{7,10}$"))
        {
            throw new ArgumentException("El NIT debe tener entre 7 y 10 dígitos");
        }
        
        // Validar teléfono si se proporciona (8 dígitos)
        if (!string.IsNullOrWhiteSpace(request.Telefono))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Telefono, @"^\d{8}$"))
            {
                throw new ArgumentException("El teléfono debe tener exactamente 8 dígitos");
            }
        }
        
        // Validar dirección si se proporciona (máximo 50 caracteres)
        if (!string.IsNullOrWhiteSpace(request.Direccion) && request.Direccion.Length > 50)
        {
            throw new ArgumentException("La dirección no puede exceder 50 caracteres");
        }

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

    /// <summary>
    /// Valida un email según las reglas:
    /// - Antes del @: solo caracteres alfanuméricos y los símbolos . - _
    /// - Los símbolos . - _ no pueden estar inmediatamente antes del @
    /// - Después del @ debe haber al menos un punto (como .com)
    /// </summary>
    private bool ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        // Verificar que tenga exactamente un @
        int arrobaIndex = email.IndexOf('@');
        if (arrobaIndex == -1 || email.IndexOf('@', arrobaIndex + 1) != -1)
        {
            return false;
        }

        // Validar parte antes del @
        string parteLocal = email.Substring(0, arrobaIndex);
        if (string.IsNullOrEmpty(parteLocal))
        {
            return false;
        }

        // Verificar que no empiece ni termine con . - _
        if (parteLocal.StartsWith(".") || parteLocal.StartsWith("-") || parteLocal.StartsWith("_") ||
            parteLocal.EndsWith(".") || parteLocal.EndsWith("-") || parteLocal.EndsWith("_"))
        {
            return false;
        }

        // Verificar que solo contenga caracteres alfanuméricos y . - _
        foreach (char c in parteLocal)
        {
            if (!char.IsLetterOrDigit(c) && c != '.' && c != '-' && c != '_')
            {
                return false;
            }
        }

        // Validar parte después del @
        string parteDominio = email.Substring(arrobaIndex + 1);
        if (string.IsNullOrEmpty(parteDominio))
        {
            return false;
        }

        // Verificar que tenga al menos un punto
        if (!parteDominio.Contains('.'))
        {
            return false;
        }

        // Verificar que el dominio tenga al menos un carácter antes del punto
        int puntoIndex = parteDominio.IndexOf('.');
        if (puntoIndex == 0)
        {
            return false;
        }

        // Verificar que después del último punto haya al menos un carácter
        int ultimoPuntoIndex = parteDominio.LastIndexOf('.');
        if (ultimoPuntoIndex == parteDominio.Length - 1)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Valida que el nombre o apellido solo contenga letras y espacios, máximo 50 caracteres
    /// </summary>
    private bool ValidarNombreApellido(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return false;
        }

        if (nombre.Length > 50)
        {
            return false;
        }

        // Solo letras y espacios (permite acentos y caracteres especiales de otros idiomas)
        foreach (char c in nombre)
        {
            if (!char.IsLetter(c) && c != ' ' && c != '-')
            {
                return false;
            }
        }

        return true;
    }
}


