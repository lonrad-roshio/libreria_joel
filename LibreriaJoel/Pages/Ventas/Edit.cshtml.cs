using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Ventas;

public class EditModel : PageModel
{
    private readonly IVentaService _ventaService;
    private readonly IProductoService _productoService;

    public EditModel(IVentaService ventaService, IProductoService productoService)
    {
        _ventaService = ventaService;
        _productoService = productoService;
    }

    [BindProperty]
    public Venta Venta { get; set; } = new();

    public List<Producto> Productos { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var venta = await _ventaService.ObtenerAsync(id);
        if (venta is null) return RedirectToPage("Index");

        Venta = venta;
        await CargarListasAsync(venta);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        ModelState.Remove("Venta.Total");

        if (!ModelState.IsValid)
        {
            var ventaOriginalParaVista = await _ventaService.ObtenerAsync(id);
            await CargarListasAsync(ventaOriginalParaVista);
            return Page();
        }

        try
        {
            var ventaOriginal = await _ventaService.ObtenerAsync(id)
                ?? throw new ValidacionException("La venta no existe.");

            await _ventaService.ActualizarAsync(Venta, ventaOriginal);
            TempData["Mensaje"] = "Venta actualizada correctamente.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var ventaOriginalParaVista = await _ventaService.ObtenerAsync(id);
            await CargarListasAsync(ventaOriginalParaVista);
            return Page();
        }
    }

    private async Task CargarListasAsync(Venta? ventaActual)
    {
        var todosProductos = await _productoService.ListarAsync(null);
        Productos = new List<Producto>();
        foreach (var producto in todosProductos)
        {
            if (producto.Estado)
                Productos.Add(producto);
        }

        Venta.Detalles = ventaActual?.Detalles ?? new List<DetalleVenta>();
    }
}
