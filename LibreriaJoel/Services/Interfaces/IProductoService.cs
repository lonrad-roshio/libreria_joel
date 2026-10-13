using LibreriaJoel.Models;

namespace LibreriaJoel.Services.Interfaces;

public interface IProductoService
{
    Task<List<Producto>> ListarAsync(string? filtro);
    Task<List<Producto>> ObtenerAlertasStockBajoAsync();
    Task<Producto?> ObtenerAsync(int id);
    Task CrearAsync(Producto producto);
    Task ActualizarAsync(Producto producto);
    Task EliminarAsync(int id);
}
