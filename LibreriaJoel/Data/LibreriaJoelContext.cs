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

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Venta> Ventas => Set<Venta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("cliente");
            entity.Property(c => c.Nombre).HasColumnType("varchar(150)");
            entity.Property(c => c.Direccion).HasColumnType("varchar(250)");
            entity.Property(c => c.Celular).HasColumnType("varchar(20)");
            entity.Property(c => c.NitCi).HasColumnType("varchar(20)");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("producto");
            entity.Property(p => p.Nombre).HasColumnType("varchar(150)");
            entity.Property(p => p.Marca).HasColumnType("varchar(80)");
            entity.Property(p => p.Lote).HasColumnType("varchar(50)");
            entity.Property(p => p.CostoEntrada).HasColumnType("decimal(10,2)");
            entity.Property(p => p.PrecioVenta).HasColumnType("decimal(10,2)");
            entity.Property(p => p.Stock).HasColumnType("decimal(10,2)");
            entity.HasIndex(p => p.Nombre);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("venta");
            entity.Property(v => v.DetalleJson).HasColumnType("json");
            entity.Property(v => v.Total).HasColumnType("decimal(10,2)");
            entity.Property(v => v.FormaPago).HasColumnType("varchar(20)");
            entity.Property(v => v.Cajero).HasColumnType("varchar(100)");

            entity.HasOne(v => v.Cliente)
                  .WithMany(c => c.Ventas)
                  .HasForeignKey(v => v.IdCliente)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
