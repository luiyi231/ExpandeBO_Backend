using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class Chat
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("clienteId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClienteId { get; set; } = string.Empty;

    [BsonElement("perfilComercialId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? PerfilComercialId { get; set; }

    [BsonElement("esSoporte")]
    public bool EsSoporte { get; set; } = false; // true si es chat con administrador/soporte

    [BsonElement("mensajes")]
    public List<Mensaje> Mensajes { get; set; } = new();

    [BsonElement("activo")]
    public bool Activo { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaUltimaActualizacion")]
    public DateTime? FechaUltimaActualizacion { get; set; }
}

public class Mensaje
{
    [BsonElement("remitenteId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string RemitenteId { get; set; } = string.Empty;

    [BsonElement("remitenteNombre")]
    public string RemitenteNombre { get; set; } = string.Empty;

    [BsonElement("remitenteRol")]
    public string RemitenteRol { get; set; } = string.Empty;

    [BsonElement("contenido")]
    public string Contenido { get; set; } = string.Empty;

    [BsonElement("fechaEnvio")]
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    [BsonElement("leido")]
    public bool Leido { get; set; } = false;
}


