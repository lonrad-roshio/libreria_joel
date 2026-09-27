using LibreriaJoel.Models;
using LibreriaJoel.Repositories;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LibreriaJoel.Services;

/// <summary>
/// Reglas de negocio para Producto, incluyendo el control de stock y las alertas de
/// "quedan pocos paquetes" que el dueño pidió expresamente en la entrevista.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repositorio;
    private readonly decimal _stockMinimoAlerta;

    public ProductoService(IProductoRepository repositorio, IConfiguration configuracion)
    {
        _repositorio = repositorio;
        _stockMinimoAlerta = configuracion.GetValue<decimal?>("ParametrosNegocio:StockMinimoAlerta") ?? 5m;
    }

    public async Task<List<Producto>> ListarAsync(string? filtro) =>
        string.IsNullOrWhiteSpace(filtro)
            ? await _repositorio.ObtenerTodosAsync()
            : await _repositorio.BuscarPorNombreOMarcaAsync(filtro.Trim());

    public async Task<Producto?> ObtenerAsync(int id) => await _repositorio.ObtenerPorIdAsync(id);

    public async Task<List<Producto>> ObtenerAlertasStockBajoAsync() =>
        await _repositorio.ObtenerConStockBajoAsync(_stockMinimoAlerta);

    public async Task CrearAsync(Producto producto)
    {
        await ValidarProductoAsync(producto);
        await _repositorio.AgregarAsync(producto);
        await _repositorio.GuardarCambiosAsync();
    }

    public async Task ActualizarAsync(Producto producto)
    {
        await ValidarProductoAsync(producto);

        if (!await _repositorio.ExisteAsync(producto.IdProducto))
            throw new ValidacionException("El producto que intenta actualizar no existe.");

        _repositorio.Actualizar(producto);
        await _repositorio.GuardarCambiosAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var producto = await _repositorio.ObtenerPorIdAsync(id)
            ?? throw new ValidacionException("El producto que intenta eliminar no existe.");

        _repositorio.Eliminar(producto);
        await _repositorio.GuardarCambiosAsync();
    }

    private async Task ValidarProductoAsync(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ValidacionException("El nombre del producto es obligatorio.");

        if (string.IsNullOrWhiteSpace(producto.Marca))
            throw new ValidacionException("La marca es obligatoria.");

        if (producto.CostoEntrada <= 0)
            throw new ValidacionException("El costo de entrada debe ser mayor a 0.");

        if (producto.PrecioVenta <= 0)
            throw new ValidacionException("El precio de venta debe ser mayor a 0.");

        if (producto.PrecioVenta < producto.CostoEntrada)
            throw new ValidacionException("El precio de venta no puede ser menor al costo de entrada.");

        if (producto.Stock < 0)
            throw new ValidacionException("El stock no puede ser negativo.");

        if (await _repositorio.NombreDuplicadoAsync(producto.Nombre, producto.Marca, producto.IdProducto))
            throw new ValidacionException("Ya existe un producto con el mismo nombre y marca.");
    }
}
