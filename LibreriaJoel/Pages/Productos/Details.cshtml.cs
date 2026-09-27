using LibreriaJoel.Models;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Productos;

public class DetailsModel : PageModel
{
    private readonly IProductoService _productoService;

    public DetailsModel(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public Producto? Producto { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Producto = await _productoService.ObtenerAsync(id);
        if (Producto is null) return RedirectToPage("Index");
        return Page();
    }
}
