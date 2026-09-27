using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Productos;

public class IndexModel : PageModel
{
    private readonly IProductoService _productoService;

    public IndexModel(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public List<Producto> Productos { get; set; } = new();
    public HashSet<int> IdsStockBajo { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Filtro { get; set; }

    public async Task OnGetAsync()
    {
        Productos = await _productoService.ListarAsync(Filtro);
        var alertas = await _productoService.ObtenerAlertasStockBajoAsync();
        IdsStockBajo = alertas.Select(p => p.IdProducto).ToHashSet();
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        try
        {
            await _productoService.EliminarAsync(id);
            TempData["Mensaje"] = "Producto eliminado correctamente.";
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}
