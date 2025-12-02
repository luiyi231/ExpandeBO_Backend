using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpandeBO_Backend.Models;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("clienteId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ClienteId { get; set; } = string.Empty;

    [BsonElement("perfilComercialId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PerfilComercialId { get; set; } = string.Empty;

    [BsonElement("items")]
    public List<PedidoItem> Items { get; set; } = new();

    [BsonElement("subtotal")]
    public decimal Subtotal { get; set; }

    [BsonElement("total")]
    public decimal Total { get; set; }

    [BsonElement("estado")]
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmado, EnPreparacion, EnCamino, Entregado, Cancelado

    [BsonElement("direccionEntrega")]
    public string? DireccionEntrega { get; set; }

    [BsonElement("latitudEntrega")]
    public double? LatitudEntrega { get; set; }

    [BsonElement("longitudEntrega")]
    public double? LongitudEntrega { get; set; }

    [BsonElement("notas")]
    public string? Notas { get; set; }

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaActualizacion")]
    public DateTime? FechaActualizacion { get; set; }
}

public class PedidoItem
{
    [BsonElement("productoId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductoId { get; set; } = string.Empty;

    [BsonElement("nombreProducto")]
    public string NombreProducto { get; set; } = string.Empty;

    [BsonElement("cantidad")]
    public int Cantidad { get; set; }

    [BsonElement("precioUnitario")]
    public decimal PrecioUnitario { get; set; }

    [BsonElement("subtotal")]
    public decimal Subtotal { get; set; }
}


