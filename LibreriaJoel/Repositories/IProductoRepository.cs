using LibreriaJoel.Models;

namespace LibreriaJoel.Repositories;

public interface IProductoRepository
{
    Task<List<Producto>> ObtenerTodosAsync();
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task<List<Producto>> BuscarPorNombreOMarcaAsync(string texto);
    Task<List<Producto>> ObtenerConStockBajoAsync(decimal umbral);
    Task AgregarAsync(Producto producto);
    void Actualizar(Producto producto);
    void Eliminar(Producto producto);
    Task<bool> ExisteAsync(int id);
    Task<bool> NombreDuplicadoAsync(string nombre, string marca, int idExcluido);
    Task GuardarCambiosAsync();
}
