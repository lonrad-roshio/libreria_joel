using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class VentaRepository : GenericRepository<Venta>, IVentaRepository
{
    public VentaRepository(DbContext database) : base(database)
    {
    }

    public override async Task<Venta?> FindAsync(int id)
    {
        return await Data
            .Include(venta => venta.Usuario)
            .Include(venta => venta.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .SingleOrDefaultAsync(venta => venta.IdVenta == id);
    }

    public async Task<List<Venta>> ListarAsync(DateTime? fecha = null)
    {
        var ventas = await Data.AsNoTracking()
            .Include(venta => venta.Usuario)
            .Include(venta => venta.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .ToListAsync();
        var resultado = fecha.HasValue
            ? ventas.FindAll(venta => venta.Fecha.Date == fecha.Value.Date)
            : ventas;

        resultado.Sort(CompararPorFecha);
        return resultado;
    }

    public async Task<List<Venta>> ObtenerEntreFechasAsync(DateTime desde, DateTime hasta)
    {
        var ventas = await Data.AsNoTracking()
            .Include(venta => venta.Usuario)
            .Include(venta => venta.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .ToListAsync();
        var resultado = ventas.FindAll(venta =>
            venta.Fecha.Date >= desde.Date && venta.Fecha.Date <= hasta.Date);

        resultado.Sort(CompararPorFecha);
        return resultado;
    }

    public async Task<decimal> ObtenerIngresosDelDiaAsync(DateTime fecha)
    {
        var ventas = await ListarAsync(fecha);
        decimal total = 0;
        foreach (var venta in ventas)
            total += venta.Total;

        return total;
    }

    private static int CompararPorFecha(Venta primera, Venta segunda)
    {
        int comparacionFecha = segunda.Fecha.CompareTo(primera.Fecha);
        if (comparacionFecha != 0)
            return comparacionFecha;
        return segunda.IdVenta.CompareTo(primera.IdVenta);
    }
}
