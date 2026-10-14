using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaJoel.Models;

[Table("detalle_venta")]
public class DetalleVenta
{
    public int IdDetalle { get; set; }

    public int IdVenta { get; set; }

    public int IdProducto { get; set; }

    public decimal Cantidad { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }

    public Venta? Venta { get; set; }

    public Producto? Producto { get; set; }
}
