using LibreriaJoel.Models;

namespace LibreriaJoel.Services.Interfaces;

public interface IClienteService
{
    Task<List<Cliente>> ListarAsync(string? filtro);
    Task<Cliente?> ObtenerAsync(int id);
    Task CrearAsync(Cliente cliente);
    Task ActualizarAsync(Cliente cliente);
    Task EliminarAsync(int id);
}
