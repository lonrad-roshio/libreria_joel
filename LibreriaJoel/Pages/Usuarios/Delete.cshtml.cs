using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Usuarios;

[Authorize(Roles = "ADMIN")]
public class DeleteModel : PageModel
{
    private readonly IUsuarioService _usuarioService;

    public DeleteModel(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public Usuario? Usuario { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Usuario = await _usuarioService.ObtenerAsync(id);
        if (Usuario is null) return RedirectToPage("Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _usuarioService.EliminarAsync(id);
            TempData["Mensaje"] = "Usuario eliminado correctamente.";
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage("Index");
    }
}
