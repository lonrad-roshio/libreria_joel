using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Productos;

public class EditModel : PageModel
{
    private readonly IProductoService _productoService;

    public EditModel(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [BindProperty]
    public Producto Producto { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var producto = await _productoService.ObtenerAsync(id);
        if (producto is null) return RedirectToPage("Index");

        Producto = producto;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _productoService.ActualizarAsync(Producto);
            TempData["Mensaje"] = "Producto actualizado correctamente.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
