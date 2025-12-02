using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [BsonElement("telefono")]
    public string? Telefono { get; set; }

    [BsonElement("rol")]
    public string Rol { get; set; } = "Cliente"; // Cliente, Empresa, Administrador

    [BsonElement("activo")]
    public bool Activo { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaUltimaActualizacion")]
    public DateTime? FechaUltimaActualizacion { get; set; }
}


