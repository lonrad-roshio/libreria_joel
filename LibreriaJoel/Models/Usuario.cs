using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaJoel.Models;

[Table("usuario")]
public class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Direccion { get; set; }

    public string? Celular { get; set; }

    public string? NitCi { get; set; }

    [Column("Contrasena")]
    public string ContrasenaHash { get; set; } = string.Empty;

    public string Rol { get; set; } = "CAJERO";

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
