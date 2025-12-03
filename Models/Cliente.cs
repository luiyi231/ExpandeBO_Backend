using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class Cliente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("usuarioId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UsuarioId { get; set; } = string.Empty;

    [BsonElement("ciudadId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CiudadId { get; set; } = string.Empty;

    [BsonElement("direccion")]
    public string? Direccion { get; set; }

    [BsonElement("activo")]
    public bool Activo { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaUltimaActualizacion")]
    public DateTime? FechaUltimaActualizacion { get; set; }
}

