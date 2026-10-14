using LibreriaJoel.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Data;

/// <summary>
/// Contexto de base de datos (EF Core) para el sistema de ventas de Librería JOEL.
/// Expone las tablas Usuario, Producto, Venta y DetalleVenta.
/// </summary>
public class LibreriaJoelContext : DbContext
{
    public LibreriaJoelContext(DbContextOptions<LibreriaJoelContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<Venta> Ventas { get; set; } = null!;
    public DbSet<DetalleVenta> DetallesVenta { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(usuario => usuario.IdUsuario);
            entity.Property(usuario => usuario.NombreUsuario).HasMaxLength(50).IsRequired();
            entity.HasIndex(usuario => usuario.NombreUsuario).IsUnique();
            entity.Property(usuario => usuario.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(usuario => usuario.Direccion).HasMaxLength(250);
            entity.Property(usuario => usuario.Celular).HasMaxLength(20);
            entity.Property(usuario => usuario.NitCi).HasMaxLength(20);
            entity.Property(usuario => usuario.ContrasenaHash).HasColumnName("Contrasena").HasMaxLength(255).IsRequired();
            entity.Property(usuario => usuario.Rol).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(producto => producto.IdProducto);
            entity.Property(producto => producto.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(producto => producto.Lote).HasMaxLength(50);
            entity.Property(producto => producto.Marca).HasMaxLength(80).IsRequired();
            entity.Property(producto => producto.UnidadMedida).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(venta => venta.IdVenta);
            entity.Property(venta => venta.FormaPago).HasMaxLength(20).IsRequired();
            entity.HasOne(venta => venta.Usuario)
                .WithMany(usuario => usuario.Ventas)
                .HasForeignKey(venta => venta.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(venta => venta.Detalles)
                .WithOne(detalle => detalle.Venta)
                .HasForeignKey(detalle => detalle.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(detalle => detalle.IdDetalle);
            entity.Property(detalle => detalle.Cantidad).HasColumnType("decimal(10,2)");
            entity.Property(detalle => detalle.PrecioUnitario).HasColumnType("decimal(10,2)");
            entity.Property(detalle => detalle.Subtotal).HasColumnType("decimal(10,2)");
            entity.HasOne(detalle => detalle.Producto)
                .WithMany(producto => producto.DetallesVenta)
                .HasForeignKey(detalle => detalle.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
