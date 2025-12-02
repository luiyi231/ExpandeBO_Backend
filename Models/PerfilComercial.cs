using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class PerfilComercial
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("empresaId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string EmpresaId { get; set; } = string.Empty;

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("descripcion")]
    public string? Descripcion { get; set; }

    [BsonElement("direccion")]
    public string? Direccion { get; set; }

    [BsonElement("ciudadId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CiudadId { get; set; }

    [BsonElement("telefono")]
    public string? Telefono { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("latitud")]
    public double? Latitud { get; set; }

    [BsonElement("longitud")]
    public double? Longitud { get; set; }

    [BsonElement("imagenUrl")]
    public string? ImagenUrl { get; set; }

    [BsonElement("horarioApertura")]
    public string? HorarioApertura { get; set; }

    [BsonElement("horarioCierre")]
    public string? HorarioCierre { get; set; }

    [BsonElement("activo")]
    public bool Activo { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaUltimaActualizacion")]
    public DateTime? FechaUltimaActualizacion { get; set; }
}


