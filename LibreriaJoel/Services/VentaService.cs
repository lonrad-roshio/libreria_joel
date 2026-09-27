using ClosedXML.Excel;
using LibreriaJoel.Models;
using LibreriaJoel.Repositories;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;

namespace LibreriaJoel.Services;

/// <summary>
/// Reglas de negocio de Venta: valida el detalle, descuenta stock (resolviendo el problema
/// de control de inventario planteado en la entrevista, incluyendo venta de unidades sueltas)
/// y genera el reporte de ventas en Excel que el dueño pidió al final del día.
/// </summary>
public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventaRepositorio;
    private readonly IProductoRepository _productoRepositorio;
    private readonly IClienteRepository _clienteRepositorio;

    public VentaService(
        IVentaRepository ventaRepositorio,
        IProductoRepository productoRepositorio,
        IClienteRepository clienteRepositorio)
    {
        _ventaRepositorio = ventaRepositorio;
        _productoRepositorio = productoRepositorio;
        _clienteRepositorio = clienteRepositorio;
    }

    public async Task<List<Venta>> ListarAsync(DateTime? fecha) =>
        fecha.HasValue
            ? await _ventaRepositorio.ObtenerPorFechaAsync(fecha.Value)
            : await _ventaRepositorio.ObtenerTodasAsync();

    public async Task<Venta?> ObtenerAsync(int id) => await _ventaRepositorio.ObtenerPorIdAsync(id);

    public async Task<Venta> RegistrarVentaAsync(Venta venta)
    {
        if (!await _clienteRepositorio.ExisteAsync(venta.IdCliente))
            throw new ValidacionException("Debe seleccionar un cliente válido.");

        var detalle = venta.Detalle;
        if (detalle.Count == 0)
            throw new ValidacionException("La venta debe tener al menos un producto.");

        decimal total = 0;

        foreach (var item in detalle)
        {
            var producto = await _productoRepositorio.ObtenerPorIdAsync(item.IdProducto)
                ?? throw new ValidacionException($"El producto con Id {item.IdProducto} no existe.");

            if (!producto.Estado)
                throw new ValidacionException($"El producto '{producto.Nombre}' no está disponible para la venta.");

            if (item.Cantidad <= 0)
                throw new ValidacionException($"La cantidad de '{producto.Nombre}' debe ser mayor a 0.");

            if (item.Cantidad > producto.Stock)
                throw new ValidacionException(
                    $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {item.Cantidad}.");

            item.NombreProducto = producto.Nombre;
            item.PrecioUnitario = producto.PrecioVenta;
            total += item.Subtotal;

            producto.Stock -= item.Cantidad;
            _productoRepositorio.Actualizar(producto);
        }

        venta.Detalle = detalle;
        venta.Total = Math.Round(total, 2);
        venta.Fecha = venta.Fecha == default ? DateTime.Today : venta.Fecha;

        await _ventaRepositorio.AgregarAsync(venta);
        await _ventaRepositorio.GuardarCambiosAsync();
        return venta;
    }

    public async Task ActualizarAsync(Venta ventaActualizada, Venta ventaOriginal)
    {
        // Devolver el stock de la venta original antes de aplicar los nuevos valores.
        foreach (var item in ventaOriginal.Detalle)
        {
            var producto = await _productoRepositorio.ObtenerPorIdAsync(item.IdProducto);
            if (producto is not null)
            {
                producto.Stock += item.Cantidad;
                _productoRepositorio.Actualizar(producto);
            }
        }

        var nuevoDetalle = ventaActualizada.Detalle;
        if (nuevoDetalle.Count == 0)
            throw new ValidacionException("La venta debe tener al menos un producto.");

        decimal total = 0;
        foreach (var item in nuevoDetalle)
        {
            var producto = await _productoRepositorio.ObtenerPorIdAsync(item.IdProducto)
                ?? throw new ValidacionException($"El producto con Id {item.IdProducto} no existe.");

            if (item.Cantidad > producto.Stock)
                throw new ValidacionException(
                    $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {item.Cantidad}.");

            item.NombreProducto = producto.Nombre;
            item.PrecioUnitario = producto.PrecioVenta;
            total += item.Subtotal;

            producto.Stock -= item.Cantidad;
            _productoRepositorio.Actualizar(producto);
        }

        ventaOriginal.Fecha = ventaActualizada.Fecha;
        ventaOriginal.IdCliente = ventaActualizada.IdCliente;
        ventaOriginal.FormaPago = ventaActualizada.FormaPago;
        ventaOriginal.Cajero = ventaActualizada.Cajero;
        ventaOriginal.Detalle = nuevoDetalle;
        ventaOriginal.Total = Math.Round(total, 2);

        _ventaRepositorio.Actualizar(ventaOriginal);
        await _ventaRepositorio.GuardarCambiosAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var venta = await _ventaRepositorio.ObtenerPorIdAsync(id)
            ?? throw new ValidacionException("La venta que intenta eliminar no existe.");

        // Devolver stock al inventario antes de eliminar (evita perder el control del inventario).
        foreach (var item in venta.Detalle)
        {
            var producto = await _productoRepositorio.ObtenerPorIdAsync(item.IdProducto);
            if (producto is not null)
            {
                producto.Stock += item.Cantidad;
                _productoRepositorio.Actualizar(producto);
            }
        }

        _ventaRepositorio.Eliminar(venta);
        await _ventaRepositorio.GuardarCambiosAsync();
    }

    public async Task<decimal> ObtenerIngresosDelDiaAsync(DateTime fecha)
    {
        var ventas = await _ventaRepositorio.ObtenerPorFechaAsync(fecha);
        return ventas.Sum(v => v.Total);
    }

    public async Task<byte[]> GenerarReporteExcelAsync(DateTime desde, DateTime hasta)
    {
        var ventas = (await _ventaRepositorio.ObtenerTodasAsync())
            .Where(v => v.Fecha.Date >= desde.Date && v.Fecha.Date <= hasta.Date)
            .ToList();

        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Reporte de Ventas");

        hoja.Cell(1, 1).Value = "Librería JOEL - Reporte de Ventas";
        hoja.Range(1, 1, 1, 7).Merge().Style.Font.SetBold().Font.FontSize = 14;

        string[] encabezados = { "Nro. Venta", "Fecha", "Cliente", "Forma de Pago", "Cajero", "Cant. Items", "Total (Bs)" };
        for (int i = 0; i < encabezados.Length; i++)
        {
            var celda = hoja.Cell(3, i + 1);
            celda.Value = encabezados[i];
            celda.Style.Font.SetBold();
            celda.Style.Fill.BackgroundColor = XLColor.FromHtml("#001F5B");
            celda.Style.Font.FontColor = XLColor.White;
        }

        int fila = 4;
        foreach (var venta in ventas)
        {
            hoja.Cell(fila, 1).Value = venta.IdVenta;
            hoja.Cell(fila, 2).Value = venta.Fecha.ToString("dd/MM/yyyy");
            hoja.Cell(fila, 3).Value = venta.Cliente?.Nombre ?? "N/D";
            hoja.Cell(fila, 4).Value = venta.FormaPago;
            hoja.Cell(fila, 5).Value = venta.Cajero;
            hoja.Cell(fila, 6).Value = venta.Detalle.Count;
            hoja.Cell(fila, 7).Value = venta.Total;
            fila++;
        }

        hoja.Cell(fila + 1, 6).Value = "TOTAL:";
        hoja.Cell(fila + 1, 6).Style.Font.SetBold();
        hoja.Cell(fila + 1, 7).Value = ventas.Sum(v => v.Total);
        hoja.Cell(fila + 1, 7).Style.Font.SetBold();

        hoja.Columns().AdjustToContents();

        using var memoria = new MemoryStream();
        libro.SaveAs(memoria);
        return memoria.ToArray();
    }
}
