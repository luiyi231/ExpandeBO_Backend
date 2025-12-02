using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class PedidoProducto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("pedidoId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PedidoId { get; set; } = string.Empty;

    [BsonElement("productoId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductoId { get; set; } = string.Empty;

    [BsonElement("cantidad")]
    public int Cantidad { get; set; }

    [BsonElement("precioUnitario")]
    public decimal PrecioUnitario { get; set; }

    [BsonElement("subtotal")]
    public decimal Subtotal { get; set; }

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

