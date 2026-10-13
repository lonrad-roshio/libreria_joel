using System.Text.Json;
using ClosedXML.Excel;
using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;

namespace LibreriaJoel.Services;

public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventas;
    private readonly IProductoRepository _productos;
    private readonly IClienteRepository _clientes;

    public VentaService(
        IVentaRepository ventas,
        IProductoRepository productos,
        IClienteRepository clientes)
    {
        _ventas = ventas;
        _productos = productos;
        _clientes = clientes;
    }

    public Task<List<Venta>> ListarAsync(DateTime? fecha = null)
    {
        return _ventas.ListarAsync(fecha);
    }

    public Task<Venta?> ObtenerAsync(int id)
    {
        return _ventas.FindAsync(id);
    }

    public Task<decimal> ObtenerIngresosDelDiaAsync(DateTime fecha)
    {
        return _ventas.ObtenerIngresosDelDiaAsync(fecha);
    }

    public async Task RegistrarVentaAsync(Venta venta)
    {
        var detalle = ObtenerDetalle(venta);
        await ValidarVentaAsync(venta, detalle);
        await AjustarStockAsync(detalle, 1);
        venta.Detalle = detalle;
        venta.Total = CalcularTotal(detalle);
        await _ventas.InsertAsync(venta);
    }

    public async Task ActualizarAsync(Venta venta, Venta ventaOriginal)
    {
        var existente = await _ventas.FindAsync(ventaOriginal.IdVenta)
            ?? throw new ValidacionException("La venta no existe.");
        var detalleNuevo = ObtenerDetalle(venta);
        var detalleAnterior = ObtenerDetalle(ventaOriginal);
        await ValidarVentaAsync(venta, detalleNuevo);

        await AjustarStockAsync(detalleAnterior, -1);
        await AjustarStockAsync(detalleNuevo, 1);
        existente.Fecha = venta.Fecha;
        existente.IdCliente = venta.IdCliente;
        existente.FormaPago = venta.FormaPago;
        existente.Cajero = venta.Cajero.Trim();
        existente.Detalle = detalleNuevo;
        existente.Total = CalcularTotal(detalleNuevo);
        await _ventas.UpdateAsync(existente);
    }

    public async Task EliminarAsync(int id)
    {
        var venta = await _ventas.FindAsync(id)
            ?? throw new ValidacionException("La venta no existe.");
        var detalle = ObtenerDetalle(venta);

        await AjustarStockAsync(detalle, -1);
        await _ventas.DeleteAsync(venta);
    }

    public async Task<byte[]> GenerarReporteExcelAsync(DateTime desde, DateTime hasta)
    {
        if (hasta.Date < desde.Date)
            throw new ValidacionException("La fecha final no puede ser anterior a la fecha inicial.");

        var ventas = await _ventas.ObtenerEntreFechasAsync(desde, hasta);
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Ventas");
        sheet.Cell(1, 1).Value = "Fecha";
        sheet.Cell(1, 2).Value = "Cliente";
        sheet.Cell(1, 3).Value = "Forma de pago";
        sheet.Cell(1, 4).Value = "Cajero";
        sheet.Cell(1, 5).Value = "Total";

        for (var index = 0; index < ventas.Count; index++)
        {
            var venta = ventas[index];
            var row = index + 2;
            sheet.Cell(row, 1).Value = venta.Fecha;
            sheet.Cell(row, 2).Value = venta.Cliente?.Nombre ?? string.Empty;
            sheet.Cell(row, 3).Value = venta.FormaPago;
            sheet.Cell(row, 4).Value = venta.Cajero;
            sheet.Cell(row, 5).Value = venta.Total;
        }

        var columnas = sheet.Columns();
        columnas.AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static List<DetalleVentaItem> ObtenerDetalle(Venta venta)
    {
        if (venta.Detalle.Count > 0)
            return venta.Detalle;

        if (string.IsNullOrWhiteSpace(venta.DetalleJson))
            return new List<DetalleVentaItem>();

        try
        {
            return JsonSerializer.Deserialize<List<DetalleVentaItem>>(venta.DetalleJson) ?? new();
        }
        catch (JsonException exception)
        {
            throw new ValidacionException($"El detalle de la venta no tiene un formato válido: {exception.Message}");
        }
    }

    private async Task ValidarVentaAsync(Venta venta, List<DetalleVentaItem> detalle)
    {
        var cliente = await _clientes.FindAsync(venta.IdCliente);
        if (cliente is null)
            throw new ValidacionException("El cliente seleccionado no existe.");
        if (detalle.Count == 0)
            throw new ValidacionException("La venta debe incluir al menos un producto.");
        foreach (var item in detalle)
        {
            if (!Validaciones.ValidarIdentificador(item.IdProducto)
                || !Validaciones.ValidarCantidad(item.Cantidad)
                || !Validaciones.ValidarPrecio(0.01m, item.PrecioUnitario))
            {
                throw new ValidacionException("Cada producto debe tener una cantidad y un precio mayores a cero.");
            }
        }
        if (string.IsNullOrWhiteSpace(venta.Cajero))
            throw new ValidacionException("El cajero es obligatorio.");
        if (string.IsNullOrWhiteSpace(venta.FormaPago))
            throw new ValidacionException("La forma de pago es obligatoria.");

        var productos = await _productos.SelectAsync();
        foreach (var item in detalle)
        {
            Producto? producto = BuscarProducto(productos, item.IdProducto);
            if (producto == null)
                throw new ValidacionException($"El producto {item.IdProducto} no existe.");
            if (!producto.Estado)
                throw new ValidacionException($"El producto '{producto.Nombre}' está inactivo.");
        }
    }

    private async Task AjustarStockAsync(List<DetalleVentaItem> detalle, int factor)
    {
        var productosProcesados = new HashSet<int>();
        foreach (var item in detalle)
        {
            if (productosProcesados.Contains(item.IdProducto))
                continue;

            productosProcesados.Add(item.IdProducto);
            var producto = await _productos.FindAsync(item.IdProducto);
            if (producto == null)
                throw new ValidacionException($"El producto {item.IdProducto} no existe.");

            decimal cantidad = 0;
            foreach (var detalleItem in detalle)
            {
                if (detalleItem.IdProducto == item.IdProducto)
                    cantidad += detalleItem.Cantidad;
            }

            var nuevoStock = producto.Stock - cantidad * factor;
            if (nuevoStock < 0)
                throw new ValidacionException($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}.");
            producto.Stock = nuevoStock;
            await _productos.UpdateAsync(producto);
        }
    }

    private static decimal CalcularTotal(List<DetalleVentaItem> detalle)
    {
        decimal total = 0;
        foreach (var item in detalle)
            total += item.Subtotal;

        return total;
    }

    private static Producto? BuscarProducto(List<Producto> productos, int id)
    {
        foreach (var producto in productos)
        {
            if (producto.IdProducto == id)
                return producto;
        }

        return null;
    }
}
