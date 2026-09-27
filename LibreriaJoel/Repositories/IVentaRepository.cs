using LibreriaJoel.Models;

namespace LibreriaJoel.Repositories;

public interface IVentaRepository
{
    Task<List<Venta>> ObtenerTodasAsync();
    Task<Venta?> ObtenerPorIdAsync(int id);
    Task<List<Venta>> ObtenerPorFechaAsync(DateTime fecha);
    Task AgregarAsync(Venta venta);
    void Actualizar(Venta venta);
    void Eliminar(Venta venta);
    Task<bool> ExisteAsync(int id);
    Task GuardarCambiosAsync();
}
