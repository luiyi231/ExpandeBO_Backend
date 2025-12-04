namespace ExpandeBO_Backend.Services;

public interface IEmpresaEliminacionService
{
    Task<bool> EliminarEmpresaConRelacionesAsync(string empresaId);
}

