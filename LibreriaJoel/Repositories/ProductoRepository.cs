using LibreriaJoel.Data;
using LibreriaJoel.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly LibreriaJoelContext _context;

    public ProductoRepository(LibreriaJoelContext context)
    {
        _context = context;
    }

    public async Task<List<Producto>> ObtenerTodosAsync() =>
        await _context.Productos.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync();

    public async Task<Producto?> ObtenerPorIdAsync(int id) =>
        await _context.Productos.FirstOrDefaultAsync(p => p.IdProducto == id);

    public async Task<List<Producto>> BuscarPorNombreOMarcaAsync(string texto) =>
        await _context.Productos.AsNoTracking()
            .Where(p => p.Nombre.Contains(texto) || p.Marca.Contains(texto))
            .OrderBy(p => p.Nombre)
            .ToListAsync();

    public async Task<List<Producto>> ObtenerConStockBajoAsync(decimal umbral) =>
        await _context.Productos.AsNoTracking()
            .Where(p => p.Estado && p.Stock <= umbral)
            .OrderBy(p => p.Stock)
            .ToListAsync();

    public async Task AgregarAsync(Producto producto) => await _context.Productos.AddAsync(producto);

    public void Actualizar(Producto producto) => _context.Productos.Update(producto);

    public void Eliminar(Producto producto) => _context.Productos.Remove(producto);

    public async Task<bool> ExisteAsync(int id) => await _context.Productos.AnyAsync(p => p.IdProducto == id);

    public async Task<bool> NombreDuplicadoAsync(string nombre, string marca, int idExcluido) =>
        await _context.Productos.AnyAsync(p =>
            p.Nombre.ToLower() == nombre.ToLower() &&
            p.Marca.ToLower() == marca.ToLower() &&
            p.IdProducto != idExcluido);

    public async Task GuardarCambiosAsync() => await _context.SaveChangesAsync();
}
