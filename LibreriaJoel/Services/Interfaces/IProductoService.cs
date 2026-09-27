using LibreriaJoel.Models;

namespace LibreriaJoel.Services.Interfaces;

public interface IProductoService
{
    Task<List<Producto>> ListarAsync(string? filtro);
    Task<Producto?> ObtenerAsync(int id);
    Task<List<Producto>> ObtenerAlertasStockBajoAsync();
    Task CrearAsync(Producto producto);
    Task ActualizarAsync(Producto producto);
    Task EliminarAsync(int id);
}
