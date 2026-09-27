using LibreriaJoel.Models;

namespace LibreriaJoel.Repositories;

/// <summary>
/// Contrato de acceso a datos para Cliente. Al depender de esta interfaz (y no de EF Core
/// directamente) las capas superiores cumplen el Principio de Inversión de Dependencias (D).
/// </summary>
public interface IClienteRepository
{
    Task<List<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task<List<Cliente>> BuscarPorNombreAsync(string texto);
    Task AgregarAsync(Cliente cliente);
    void Actualizar(Cliente cliente);
    void Eliminar(Cliente cliente);
    Task<bool> ExisteAsync(int id);
    Task<bool> TieneVentasAsociadasAsync(int idCliente);
    Task GuardarCambiosAsync();
}
