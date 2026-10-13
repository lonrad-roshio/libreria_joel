using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;

namespace LibreriaJoel.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repository;
    private readonly decimal _stockMinimo;

    public ProductoService(IProductoRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _stockMinimo = configuration.GetValue<decimal>("ParametrosNegocio:StockMinimoAlerta", 5m);
    }

    public Task<List<Producto>> ListarAsync(string? filtro)
    {
        return _repository.BuscarAsync(filtro);
    }

    public Task<List<Producto>> ObtenerAlertasStockBajoAsync()
    {
        return _repository.ObtenerStockBajoAsync(_stockMinimo);
    }

    public Task<Producto?> ObtenerAsync(int id)
    {
        return _repository.FindAsync(id);
    }

    public async Task CrearAsync(Producto producto)
    {
        await ValidarAsync(producto);
        producto.Nombre = producto.Nombre.Trim();
        producto.Marca = producto.Marca.Trim();
        producto.UnidadMedida = producto.UnidadMedida.Trim();
        await _repository.InsertAsync(producto);
    }

    public async Task ActualizarAsync(Producto producto)
    {
        var existente = await _repository.FindAsync(producto.IdProducto)
            ?? throw new ValidacionException("El producto no existe.");

        await ValidarAsync(producto, producto.IdProducto);
        existente.Nombre = producto.Nombre.Trim();
        existente.Lote = producto.Lote?.Trim();
        existente.Marca = producto.Marca.Trim();
        existente.CostoEntrada = producto.CostoEntrada;
        existente.PrecioVenta = producto.PrecioVenta;
        existente.FechaIngreso = producto.FechaIngreso;
        existente.Stock = producto.Stock;
        existente.UnidadMedida = producto.UnidadMedida.Trim();
        existente.Estado = producto.Estado;
        await _repository.UpdateAsync(existente);
    }

    public async Task EliminarAsync(int id)
    {
        var producto = await _repository.FindAsync(id)
            ?? throw new ValidacionException("El producto no existe.");
        await _repository.DeleteAsync(producto);
    }

    private async Task ValidarAsync(Producto producto, int? excluirId = null)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre) || string.IsNullOrWhiteSpace(producto.Marca))
            throw new ValidacionException("El nombre y la marca son obligatorios.");
        if (!Validaciones.ValidarPrecio(producto.CostoEntrada, producto.PrecioVenta))
            throw new ValidacionException("El precio de venta no puede ser menor al costo de entrada.");
        if (!Validaciones.ValidarStock(producto.Stock))
            throw new ValidacionException("El stock no puede ser negativo.");
        if (await _repository.ExisteNombreMarcaAsync(producto.Nombre.Trim(), producto.Marca.Trim(), excluirId))
            throw new ValidacionException("Ya existe un producto con el mismo nombre y marca.");
    }
}
