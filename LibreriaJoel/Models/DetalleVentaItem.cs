using System.ComponentModel.DataAnnotations;

namespace LibreriaJoel.Models;

public class DetalleVentaItem
{
    public int IdProducto { get; set; }

    [Required]
    public string NombreProducto { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal
    {
        get { return Cantidad * PrecioUnitario; }
    }
}
