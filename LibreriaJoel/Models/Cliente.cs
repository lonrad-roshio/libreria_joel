using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaJoel.Models;

[Table("cliente")]
public class Cliente
{
    public int IdCliente { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Direccion { get; set; }

    [StringLength(20)]
    public string? Celular { get; set; }

    [StringLength(20)]
    public string? NitCi { get; set; }

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
