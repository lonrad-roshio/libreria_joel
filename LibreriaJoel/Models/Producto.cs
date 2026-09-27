using System.ComponentModel.DataAnnotations;

namespace LibreriaJoel.Models;

/// <summary>
/// Representa un producto de material escolar/escritorio (bolígrafos, colores, cuadernos, hojas, etc.).
/// El stock se maneja en "unidades base" para resolver el problema de control de inventario
/// mencionado por el dueño (vende tanto por paquete/caja como por unidad suelta).
/// </summary>
public class Producto
{
    [Key]
    public int IdProducto { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El lote no puede superar 50 caracteres.")]
    public string? Lote { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria.")]
    [StringLength(80, ErrorMessage = "La marca no puede superar 80 caracteres.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "El costo de entrada es obligatorio.")]
    [Range(0.01, 1000000, ErrorMessage = "El costo de entrada debe ser mayor a 0.")]
    [DataType(DataType.Currency)]
    public decimal CostoEntrada { get; set; }

    [Required(ErrorMessage = "El precio de venta es obligatorio.")]
    [Range(0.01, 1000000, ErrorMessage = "El precio de venta debe ser mayor a 0.")]
    [DataType(DataType.Currency)]
    public decimal PrecioVenta { get; set; }

    [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaIngreso { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El stock es obligatorio.")]
    [Range(0, 1000000, ErrorMessage = "El stock no puede ser negativo.")]
    public decimal Stock { get; set; }

    [StringLength(20)]
    public string? UnidadMedida { get; set; } = "UNIDAD"; // UNIDAD, PAQUETE, CAJA, etc.

    // No requerido según levantamiento de requerimientos
    public bool Estado { get; set; } = true; // true = activo/disponible para la venta
}
