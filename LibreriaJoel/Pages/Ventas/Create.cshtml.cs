using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace LibreriaJoel.Pages.Ventas;

public class CreateModel : PageModel
{
    private readonly IVentaService _ventaService;
    private readonly IProductoService _productoService;

    public CreateModel(IVentaService ventaService, IProductoService productoService)
    {
        _ventaService = ventaService;
        _productoService = productoService;
    }

    [BindProperty]
    public Venta Venta { get; set; } = new();

    public List<Producto> Productos { get; set; } = new();

    public async Task OnGetAsync()
    {
        await CargarListasAsync();
        Venta.Fecha = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("Venta.Total");

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idUsuario))
            return Forbid();
        Venta.IdUsuario = idUsuario;

        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return Page();
        }

        try
        {
            await _ventaService.RegistrarVentaAsync(Venta);
            TempData["Mensaje"] = "Venta registrada correctamente y stock actualizado.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await CargarListasAsync();
            return Page();
        }
    }

    private async Task CargarListasAsync()
    {
        var todosProductos = await _productoService.ListarAsync(null);
        Productos = new List<Producto>();
        foreach (var producto in todosProductos)
        {
            if (producto.Estado && producto.Stock > 0)
                Productos.Add(producto);
        }

    }
}
