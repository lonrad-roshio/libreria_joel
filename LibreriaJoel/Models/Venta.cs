using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace LibreriaJoel.Models;

[Table("venta")]
public class Venta
{
    private List<DetalleVentaItem> _detalle = new();

    public int IdVenta { get; set; }

    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; } = DateTime.Today;

    public int IdCliente { get; set; }

    [Required]
    [Column(TypeName = "json")]
    public string DetalleJson { get; set; } = "[]";

    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "La forma de pago es obligatoria.")]
    [StringLength(20)]
    public string FormaPago { get; set; } = "EFECTIVO";

    [Required(ErrorMessage = "El cajero es obligatorio.")]
    [StringLength(100)]
    public string Cajero { get; set; } = string.Empty;

    [ForeignKey(nameof(IdCliente))]
    public Cliente? Cliente { get; set; }

    [NotMapped]
    public List<DetalleVentaItem> Detalle
    {
        get
        {
            if (_detalle.Count > 0)
                return _detalle;

            if (string.IsNullOrWhiteSpace(DetalleJson))
                return _detalle;

            _detalle = JsonSerializer.Deserialize<List<DetalleVentaItem>>(DetalleJson) ?? new();
            return _detalle;
        }
        set
        {
            _detalle = value ?? new();
            DetalleJson = JsonSerializer.Serialize(_detalle);
        }
    }
}
