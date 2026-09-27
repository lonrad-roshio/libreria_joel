using System.ComponentModel.DataAnnotations;

namespace LibreriaJoel.Models;

/// <summary>
/// Representa a un cliente de la librería. Entidad de persistencia (EF Core).
/// Cumple con el Principio de Responsabilidad Única (S de SOLID): solo modela datos del cliente,
/// ninguna regla de negocio vive aquí (eso corresponde a ClienteService).
/// </summary>
public class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "La dirección no puede superar 250 caracteres.")]
    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El celular no puede superar 20 caracteres.")]
    [RegularExpression(@"^[0-9+\s-]*$", ErrorMessage = "El celular solo puede contener números, espacios, '+' o '-'.")]
    public string? Celular { get; set; }

    [StringLength(20, ErrorMessage = "El NIT/CI no puede superar 20 caracteres.")]
    public string? NitCi { get; set; }

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
