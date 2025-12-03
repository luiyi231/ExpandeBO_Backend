namespace ExpandeBO_Backend.Models.DTOs;

public class UpdateRepartidorRequest
{
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public double? ProgresoRuta { get; set; } // 0.0 a 1.0
    public string? RutaGeometry { get; set; } // Polyline codificado
    public int? DuracionRutaSegundos { get; set; }
}