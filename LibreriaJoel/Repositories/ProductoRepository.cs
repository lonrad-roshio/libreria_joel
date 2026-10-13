using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
{
    public ProductoRepository(DbContext database) : base(database)
    {
    }

    public Task<List<Producto>> BuscarAsync(string? filtro)
    {
        return BuscarProductosAsync(filtro);
    }

    private async Task<List<Producto>> BuscarProductosAsync(string? filtro)
    {
        var productos = await SelectAsync();
        var resultado = new List<Producto>();
        string texto = string.Empty;

        if (!string.IsNullOrWhiteSpace(filtro))
            texto = filtro.Trim();

        foreach (var producto in productos)
        {
            if (texto.Length == 0
                || producto.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)
                || producto.Marca.Contains(texto, StringComparison.OrdinalIgnoreCase))
            {
                resultado.Add(producto);
            }
        }

        resultado.Sort(CompararPorNombre);
        return resultado;
    }

    public async Task<List<Producto>> ObtenerStockBajoAsync(decimal stockMinimo)
    {
        var productos = await SelectAsync();
        var resultado = new List<Producto>();
        foreach (var producto in productos)
        {
            if (producto.Estado && producto.Stock <= stockMinimo)
                resultado.Add(producto);
        }

        resultado.Sort(CompararPorStock);
        return resultado;
    }

    public async Task<bool> ExisteNombreMarcaAsync(
        string nombre,
        string marca,
        int? excluirId = null)
    {
        var productos = await SelectAsync();
        foreach (var producto in productos)
        {
            if (producto.Nombre == nombre
                && producto.Marca == marca
                && producto.IdProducto != excluirId)
            {
                return true;
            }
        }

        return false;
    }

    private static int CompararPorNombre(Producto primero, Producto segundo)
    {
        return string.Compare(primero.Nombre, segundo.Nombre, StringComparison.OrdinalIgnoreCase);
    }

    private static int CompararPorStock(Producto primero, Producto segundo)
    {
        return primero.Stock.CompareTo(segundo.Stock);
    }
}
