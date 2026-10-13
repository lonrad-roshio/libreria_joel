using LibreriaJoel.Models;

namespace LibreriaJoel.Repositories.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<bool> TieneVentasAsync(int idCliente);
}
