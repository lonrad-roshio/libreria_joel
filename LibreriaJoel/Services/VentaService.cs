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
    private readonly IUsuarioRepository _usuarios;

    public VentaService(
        IVentaRepository ventas,
        IProductoRepository productos,
        IUsuarioRepository usuarios)
    {
        _ventas = ventas;
        _productos = productos;
        _usuarios = usuarios;
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
        var detalle = venta.Detalles.ToList();
        await ValidarVentaAsync(venta, detalle);
        await AjustarStockAsync(detalle, 1);
        venta.Detalles = detalle;
        venta.Total = CalcularTotal(detalle);
        await _ventas.InsertAsync(venta);
    }

    public async Task ActualizarAsync(Venta venta, Venta ventaOriginal)
    {
        var existente = await _ventas.FindAsync(ventaOriginal.IdVenta)
            ?? throw new ValidacionException("La venta no existe.");
        var detalleNuevo = venta.Detalles.ToList();
        var detalleAnterior = existente.Detalles.ToList();
        venta.IdUsuario = existente.IdUsuario;
        await ValidarVentaAsync(venta, detalleNuevo);

        await AjustarStockAsync(detalleAnterior, -1);
        await AjustarStockAsync(detalleNuevo, 1);
        existente.Fecha = venta.Fecha;
        existente.FormaPago = venta.FormaPago;
        existente.Detalles.Clear();
        foreach (var item in detalleNuevo)
        {
            item.IdDetalle = 0;
            item.IdVenta = existente.IdVenta;
            existente.Detalles.Add(item);
        }
        existente.Total = CalcularTotal(detalleNuevo);
        await _ventas.UpdateAsync(existente);
    }

    public async Task EliminarAsync(int id)
    {
        var venta = await _ventas.FindAsync(id)
            ?? throw new ValidacionException("La venta no existe.");

        await AjustarStockAsync(venta.Detalles.ToList(), -1);
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
        sheet.Cell(1, 2).Value = "Nombre del usuario";
        sheet.Cell(1, 3).Value = "Forma de pago";
        sheet.Cell(1, 4).Value = "Nombre de usuario";
        sheet.Cell(1, 5).Value = "Total";

        for (var index = 0; index < ventas.Count; index++)
        {
            var venta = ventas[index];
            var row = index + 2;
            sheet.Cell(row, 1).Value = venta.Fecha;
            sheet.Cell(row, 2).Value = venta.Usuario?.Nombre ?? string.Empty;
            sheet.Cell(row, 3).Value = venta.FormaPago;
            sheet.Cell(row, 4).Value = venta.Usuario?.NombreUsuario ?? string.Empty;
            sheet.Cell(row, 5).Value = venta.Total;
        }

        var columnas = sheet.Columns();
        columnas.AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private async Task ValidarVentaAsync(Venta venta, List<DetalleVenta> detalle)
    {
        venta.Detalles = detalle;
        var errorVenta = Validaciones.ValidarVenta(venta);
        if (errorVenta is not null)
            throw new ValidacionException(errorVenta);

        if (!Validaciones.ValidarIdentificador(venta.IdUsuario)
            || await _usuarios.FindAsync(venta.IdUsuario) is null)
            throw new ValidacionException("El usuario que registra la venta no existe.");

        var productos = await _productos.SelectAsync();
        foreach (var item in detalle)
        {
            var errorDetalle = Validaciones.ValidarDetalleVenta(item);
            if (errorDetalle is not null)
                throw new ValidacionException(errorDetalle);

            Producto? producto = BuscarProducto(productos, item.IdProducto);
            if (producto == null)
                throw new ValidacionException($"El producto {item.IdProducto} no existe.");
            if (!producto.Estado)
                throw new ValidacionException($"El producto '{producto.Nombre}' está inactivo.");

            item.PrecioUnitario = producto.PrecioVenta;
            item.Subtotal = item.Cantidad * item.PrecioUnitario;
        }
    }

    private async Task AjustarStockAsync(List<DetalleVenta> detalle, int factor)
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

    private static decimal CalcularTotal(List<DetalleVenta> detalle)
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
