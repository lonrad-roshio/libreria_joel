using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Productos;

public class DeleteModel : PageModel
{
    private readonly IProductoService _productoService;

    public DeleteModel(IProductoService productoService)
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

    public async Task<IActionResult> OnPostAsync(int id)
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
        return RedirectToPage("Index");
    }
}
