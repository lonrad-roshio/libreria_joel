using LibreriaJoel.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Data;

/// <summary>
/// Contexto de base de datos (EF Core) para el sistema de ventas de Librería JOEL.
/// Solo expone las 3 tablas permitidas por la consigna del trabajo práctico:
/// Cliente, Producto y Venta.
/// </summary>
public class LibreriaJoelContext : DbContext
{
    public LibreriaJoelContext(DbContextOptions<LibreriaJoelContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Venta> Ventas { get; set; } = null!;
}
