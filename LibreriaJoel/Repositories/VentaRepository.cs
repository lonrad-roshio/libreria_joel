using LibreriaJoel.Data;
using LibreriaJoel.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly LibreriaJoelContext _context;

    public VentaRepository(LibreriaJoelContext context)
    {
        _context = context;
    }

    public async Task<List<Venta>> ObtenerTodasAsync() =>
        await _context.Ventas.AsNoTracking()
            .Include(v => v.Cliente)
            .OrderByDescending(v => v.Fecha)
            .ThenByDescending(v => v.IdVenta)
            .ToListAsync();

    public async Task<Venta?> ObtenerPorIdAsync(int id) =>
        await _context.Ventas.Include(v => v.Cliente).FirstOrDefaultAsync(v => v.IdVenta == id);

    public async Task<List<Venta>> ObtenerPorFechaAsync(DateTime fecha) =>
        await _context.Ventas.AsNoTracking()
            .Include(v => v.Cliente)
            .Where(v => v.Fecha.Date == fecha.Date)
            .OrderByDescending(v => v.IdVenta)
            .ToListAsync();

    public async Task AgregarAsync(Venta venta) => await _context.Ventas.AddAsync(venta);

    public void Actualizar(Venta venta) => _context.Ventas.Update(venta);

    public void Eliminar(Venta venta) => _context.Ventas.Remove(venta);

    public async Task<bool> ExisteAsync(int id) => await _context.Ventas.AnyAsync(v => v.IdVenta == id);

    public async Task GuardarCambiosAsync() => await _context.SaveChangesAsync();
}
