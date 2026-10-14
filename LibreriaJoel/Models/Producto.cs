using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaJoel.Models;

[Table("producto")]
public class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Lote { get; set; }

    public string Marca { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal CostoEntrada { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioVenta { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaIngreso { get; set; } = DateTime.Today;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Stock { get; set; }

    public string UnidadMedida { get; set; } = "UNIDAD";

    public bool Estado { get; set; } = true;

    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
}
