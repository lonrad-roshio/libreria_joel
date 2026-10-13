using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace LibreriaJoel.Pages.Ventas;

public class EditModel : PageModel
{
    private static readonly JsonSerializerOptions OpcionesJson = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IVentaService _ventaService;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;

    public EditModel(IVentaService ventaService, IClienteService clienteService, IProductoService productoService)
    {
        _ventaService = ventaService;
        _clienteService = clienteService;
        _productoService = productoService;
    }

    [BindProperty]
    public Venta Venta { get; set; } = new();

    public List<SelectListItem> Clientes { get; set; } = new();
    public List<Producto> Productos { get; set; } = new();
    public List<DetalleVentaItem> DetalleInicial { get; set; } = new();
    public string ProductosJson { get; set; } = "[]";
    public string DetalleInicialJson { get; set; } = "[]";

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var venta = await _ventaService.ObtenerAsync(id);
        if (venta is null) return RedirectToPage("Index");

        Venta = venta;
        DetalleInicial = venta.Detalle;
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
        var clientes = await _clienteService.ListarAsync(null);
        Clientes = new List<SelectListItem>();
        foreach (var cliente in clientes)
            Clientes.Add(new SelectListItem(cliente.Nombre, cliente.IdCliente.ToString()));

        var todosProductos = await _productoService.ListarAsync(null);
        Productos = new List<Producto>();
        foreach (var producto in todosProductos)
        {
            if (producto.Estado)
                Productos.Add(producto);
        }

        DetalleInicial = ventaActual?.Detalle ?? new List<DetalleVentaItem>();
        ProductosJson = JsonSerializer.Serialize(Productos, OpcionesJson);
        DetalleInicialJson = JsonSerializer.Serialize(DetalleInicial, OpcionesJson);
    }
}
