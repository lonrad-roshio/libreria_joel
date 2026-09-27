using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Ventas;

public class CreateModel : PageModel
{
    private readonly IVentaService _ventaService;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;

    public CreateModel(IVentaService ventaService, IClienteService clienteService, IProductoService productoService)
    {
        _ventaService = ventaService;
        _clienteService = clienteService;
        _productoService = productoService;
    }

    [BindProperty]
    public Venta Venta { get; set; } = new();

    public List<SelectListItem> Clientes { get; set; } = new();
    public List<Producto> Productos { get; set; } = new();

    public async Task OnGetAsync()
    {
        await CargarListasAsync();
        Venta.Fecha = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // El detalle (lista de productos) llega serializado en Venta.DetalleJson desde el JS
        // del formulario (ver wwwroot/js/ventas.js); no se valida con [Required] estándar
        // porque su contenido es dinámico, se valida explícitamente en el servicio.
        ModelState.Remove("Venta.Total");

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
        var clientes = await _clienteService.ListarAsync(null);
        Clientes = clientes.Select(c => new SelectListItem($"{c.Nombre}", c.IdCliente.ToString())).ToList();

        Productos = (await _productoService.ListarAsync(null)).Where(p => p.Estado && p.Stock > 0).ToList();
    }
}
