using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Controllers;

// Clases auxiliares para el seeder de empresas
public class EmpresaSeedData
{
    public string CiudadNombre { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NIT { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string PlanNombre { get; set; } = string.Empty;
    public PerfilSeedData Perfil { get; set; } = new();
    public List<ProductoSeedData> Productos { get; set; } = new();
}

public class PerfilSeedData
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public string HorarioApertura { get; set; } = string.Empty;
    public string HorarioCierre { get; set; } = string.Empty;
}

public class ProductoSeedData
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public string SubcategoriaNombre { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
}

