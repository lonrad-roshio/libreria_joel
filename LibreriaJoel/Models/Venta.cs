using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace LibreriaJoel.Models;

/// <summary>
/// Opciones de serialización compartidas para el detalle de venta (JSON). Se usa camelCase
/// porque así lo genera el JavaScript del formulario (wwwroot/js/ventas.js) y así se guardó
/// también en los datos de ejemplo del script SQL; PropertyNameCaseInsensitive agrega una
/// capa extra de seguridad por si en algún punto llega el JSON en otra convención de mayúsculas.
/// BUG CORREGIDO: antes se usaba JsonSerializer.Serialize/Deserialize sin estas opciones, lo
/// que hacía que las claves en minúscula del JS (idProducto, cantidad, etc.) no calzaran con
/// las propiedades en PascalCase de DetalleVentaItem (IdProducto, Cantidad, etc.), y por eso
/// el detalle de la venta se guardaba en cero / no se veía en la pantalla de detalle.
/// </summary>
internal static class OpcionesJsonVenta
{
    public static readonly JsonSerializerOptions Instancia = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}

/// <summary>
/// Representa una venta (recibo interno; el dueño confirmó en la entrevista que NO se emite factura).
/// La lista de productos vendidos se guarda en la columna JSON DetalleJson (ver DetalleVentaItem).
/// </summary>
public class Venta
{
    [Key]
    public int IdVenta { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Debe indicar el cliente.")]
    [ForeignKey(nameof(Cliente))]
    public int IdCliente { get; set; }
    public Cliente? Cliente { get; set; }

    [Required(ErrorMessage = "Debe registrar al menos un producto en la venta.")]
    public string DetalleJson { get; set; } = "[]";

    [Required(ErrorMessage = "El total es obligatorio.")]
    [Range(0.01, 10000000, ErrorMessage = "El total debe ser mayor a 0.")]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "La forma de pago es obligatoria.")]
    [StringLength(20)]
    public string FormaPago { get; set; } = "EFECTIVO"; // EFECTIVO o QR, según entrevista

    [Required(ErrorMessage = "El cajero que atiende es obligatorio.")]
    [StringLength(100)]
    public string Cajero { get; set; } = string.Empty;

    [NotMapped]
    public List<DetalleVentaItem> Detalle
    {
        get => string.IsNullOrWhiteSpace(DetalleJson)
            ? new List<DetalleVentaItem>()
            : JsonSerializer.Deserialize<List<DetalleVentaItem>>(DetalleJson, OpcionesJsonVenta.Instancia) ?? new List<DetalleVentaItem>();
        set => DetalleJson = JsonSerializer.Serialize(value, OpcionesJsonVenta.Instancia);
    }
}
