using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class Producto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("perfilComercialId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PerfilComercialId { get; set; } = string.Empty;

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("descripcion")]
    public string? Descripcion { get; set; }

    [BsonElement("precio")]
    public decimal Precio { get; set; }

    [BsonElement("categoriaId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? CategoriaId { get; set; }

    [BsonElement("subcategoriaId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? SubcategoriaId { get; set; }

    [BsonElement("imagenUrl")]
    public string? ImagenUrl { get; set; }

    [BsonElement("stock")]
    public int Stock { get; set; } = 0;

    [BsonElement("disponible")]
    public bool Disponible { get; set; } = true;

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaUltimaActualizacion")]
    public DateTime? FechaUltimaActualizacion { get; set; }
}


