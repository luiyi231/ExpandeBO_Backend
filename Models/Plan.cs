using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class Plan
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("descripcion")]
    public string? Descripcion { get; set; }

    [BsonElement("precio")]
    public decimal Precio { get; set; }

    [BsonElement("duracionDias")]
    public int DuracionDias { get; set; } // 30, 90, 365, etc.

    [BsonElement("maxPerfilesComerciales")]
    public int MaxPerfilesComerciales { get; set; } = 1;

    [BsonElement("maxProductos")]
    public int MaxProductos { get; set; } = 100;

    [BsonElement("activo")]
    public bool Activo { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}


