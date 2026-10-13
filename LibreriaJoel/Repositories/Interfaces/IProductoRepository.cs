using LibreriaJoel.Models;

namespace LibreriaJoel.Repositories.Interfaces;

public interface IProductoRepository : IRepository<Producto>
{
    Task<List<Producto>> BuscarAsync(string? filtro);
    Task<List<Producto>> ObtenerStockBajoAsync(decimal stockMinimo);
    Task<bool> ExisteNombreMarcaAsync(string nombre, string marca, int? excluirId = null);
}
