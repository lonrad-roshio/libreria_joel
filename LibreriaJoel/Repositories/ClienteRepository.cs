using LibreriaJoel.Data;
using LibreriaJoel.Models;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly LibreriaJoelContext _context;

    public ClienteRepository(LibreriaJoelContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> ObtenerTodosAsync() =>
        await _context.Clientes.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();

    public async Task<Cliente?> ObtenerPorIdAsync(int id) =>
        await _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == id);

    public async Task<List<Cliente>> BuscarPorNombreAsync(string texto) =>
        await _context.Clientes.AsNoTracking()
            .Where(c => c.Nombre.Contains(texto))
            .OrderBy(c => c.Nombre)
            .ToListAsync();

    public async Task AgregarAsync(Cliente cliente) => await _context.Clientes.AddAsync(cliente);

    public void Actualizar(Cliente cliente) => _context.Clientes.Update(cliente);

    public void Eliminar(Cliente cliente) => _context.Clientes.Remove(cliente);

    public async Task<bool> ExisteAsync(int id) => await _context.Clientes.AnyAsync(c => c.IdCliente == id);

    public async Task<bool> TieneVentasAsociadasAsync(int idCliente) =>
        await _context.Ventas.AnyAsync(v => v.IdCliente == idCliente);

    public async Task GuardarCambiosAsync() => await _context.SaveChangesAsync();
}
