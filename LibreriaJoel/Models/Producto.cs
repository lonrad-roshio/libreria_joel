using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaJoel.Models;

[Table("producto")]
public class Producto
{
    public int IdProducto { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Lote { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal CostoEntrada { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioVenta { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaIngreso { get; set; } = DateTime.Today;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Stock { get; set; }

    [Required]
    [StringLength(20)]
    public string UnidadMedida { get; set; } = "UNIDAD";

    public bool Estado { get; set; } = true;
}
