using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class SuscripcionEmpresa
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("empresaId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string EmpresaId { get; set; } = string.Empty;

    [BsonElement("planId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PlanId { get; set; } = string.Empty;

    [BsonElement("fechaInicio")]
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaFin")]
    public DateTime FechaFin { get; set; }

    [BsonElement("activa")]
    public bool Activa { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}


