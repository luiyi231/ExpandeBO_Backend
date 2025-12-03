using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;

namespace ExpandeBO_Backend.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ICiudadRepository _ciudadRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ClienteService(
        IClienteRepository clienteRepository,
        ICiudadRepository ciudadRepository,
        IUsuarioRepository usuarioRepository)
    {
        _clienteRepository = clienteRepository;
        _ciudadRepository = ciudadRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Cliente> CreateClienteAsync(string usuarioId, string ciudadId, string? direccion = null)
    {
        // Validar que el usuario existe
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado");
        }

        // Validar que la ciudad existe
        var ciudad = await _ciudadRepository.GetByIdAsync(ciudadId);
        if (ciudad == null || !ciudad.Activa)
        {
            throw new KeyNotFoundException("Ciudad no encontrada o inactiva");
        }

        // Verificar que no exista ya un cliente para este usuario
        var clienteExistente = await _clienteRepository.GetByUsuarioIdAsync(usuarioId);
        if (clienteExistente != null)
        {
            throw new InvalidOperationException("Ya existe un cliente para este usuario");
        }

        var cliente = new Cliente
        {
            UsuarioId = usuarioId,
            CiudadId = ciudadId,
            Direccion = direccion,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        return await _clienteRepository.CreateAsync(cliente);
    }

    public async Task<Cliente?> GetClienteByUsuarioIdAsync(string usuarioId)
    {
        return await _clienteRepository.GetByUsuarioIdAsync(usuarioId);
    }

    public async Task<Cliente?> GetClienteByIdAsync(string id)
    {
        return await _clienteRepository.GetByIdAsync(id);
    }

    public async Task<Cliente> UpdateClienteAsync(string clienteId, string ciudadId, string? direccion = null)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        if (cliente == null)
        {
            throw new KeyNotFoundException("Cliente no encontrado");
        }

        // Validar que la ciudad existe
        var ciudad = await _ciudadRepository.GetByIdAsync(ciudadId);
        if (ciudad == null || !ciudad.Activa)
        {
            throw new KeyNotFoundException("Ciudad no encontrada o inactiva");
        }

        cliente.CiudadId = ciudadId;
        if (direccion != null)
        {
            cliente.Direccion = direccion;
        }

        return await _clienteRepository.UpdateAsync(cliente);
    }

    public async Task<List<Cliente>> GetClientesByCiudadAsync(string ciudadId)
    {
        return await _clienteRepository.GetByCiudadIdAsync(ciudadId);
    }
}

