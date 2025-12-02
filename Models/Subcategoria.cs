using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class Subcategoria
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("categoriaId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CategoriaId { get; set; } = string.Empty;

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("descripcion")]
    public string? Descripcion { get; set; }

    [BsonElement("activa")]
    public bool Activa { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}


