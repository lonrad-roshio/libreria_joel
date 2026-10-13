using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class VentaRepository : GenericRepository<Venta>, IVentaRepository
{
    private readonly IRepository<Cliente> _clientes;

    public VentaRepository(DbContext database, IRepository<Cliente> clientes) : base(database)
    {
        _clientes = clientes;
    }

    public override async Task<Venta?> FindAsync(int id)
    {
        var venta = await Data.FindAsync(id);
        if (venta == null)
            return null;

        venta.Cliente = await _clientes.FindAsync(venta.IdCliente);
        return venta;
    }

    public async Task<List<Venta>> ListarAsync(DateTime? fecha = null)
    {
        var ventas = await SelectAsync();
        var clientes = await ObtenerClientesAsync();
        var resultado = new List<Venta>();

        foreach (var venta in ventas)
        {
            if (!fecha.HasValue || venta.Fecha.Date == fecha.Value.Date)
            {
                venta.Cliente = BuscarCliente(clientes, venta.IdCliente);
                resultado.Add(venta);
            }
        }

        resultado.Sort(CompararPorFecha);
        return resultado;
    }

    public async Task<List<Venta>> ObtenerEntreFechasAsync(DateTime desde, DateTime hasta)
    {
        var ventas = await SelectAsync();
        var clientes = await ObtenerClientesAsync();
        var resultado = new List<Venta>();
        foreach (var venta in ventas)
        {
            if (venta.Fecha.Date >= desde.Date && venta.Fecha.Date <= hasta.Date)
            {
                venta.Cliente = BuscarCliente(clientes, venta.IdCliente);
                resultado.Add(venta);
            }
        }

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

    private async Task<List<Cliente>> ObtenerClientesAsync()
    {
        return await _clientes.SelectAsync();
    }

    private static Cliente? BuscarCliente(List<Cliente> clientes, int idCliente)
    {
        foreach (var cliente in clientes)
        {
            if (cliente.IdCliente == idCliente)
                return cliente;
        }

        return null;
    }

    private static int CompararPorFecha(Venta primera, Venta segunda)
    {
        int comparacionFecha = segunda.Fecha.CompareTo(primera.Fecha);
        if (comparacionFecha != 0)
            return comparacionFecha;
        return segunda.IdVenta.CompareTo(primera.IdVenta);
    }
}
