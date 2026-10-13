using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
{
    private readonly IRepository<Venta> _ventas;

    public ClienteRepository(DbContext database, IRepository<Venta> ventas) : base(database)
    {
        _ventas = ventas;
    }

    public async Task<bool> TieneVentasAsync(int idCliente)
    {
        var ventas = await _ventas.SelectAsync();
        foreach (var venta in ventas)
        {
            if (venta.IdCliente == idCliente)
                return true;
        }

        return false;
    }
}
