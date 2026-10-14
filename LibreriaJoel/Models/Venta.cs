using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaJoel.Models;

[Table("venta")]
public class Venta
{
    public int IdVenta { get; set; }

    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    public int IdUsuario { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    public string FormaPago { get; set; } = "EFECTIVO";

    public Usuario? Usuario { get; set; }

    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
