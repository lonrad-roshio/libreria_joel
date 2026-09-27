using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Ventas;

public class DeleteModel : PageModel
{
    private readonly IVentaService _ventaService;

    public DeleteModel(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    public Venta? Venta { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Venta = await _ventaService.ObtenerAsync(id);
        if (Venta is null) return RedirectToPage("Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _ventaService.EliminarAsync(id);
            TempData["Mensaje"] = "Venta eliminada y stock restituido correctamente.";
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage("Index");
    }
}
