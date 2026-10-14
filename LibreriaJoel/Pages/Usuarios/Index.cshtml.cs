using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Usuarios;

[Authorize(Roles = "ADMIN")]
public class IndexModel : PageModel
{
    private readonly IUsuarioService _usuarioService;

    public IndexModel(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public List<Usuario> Usuarios { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Filtro { get; set; }

    public async Task OnGetAsync()
    {
        Usuarios = await _usuarioService.ListarAsync(Filtro);
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
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
        return RedirectToPage();
    }
}
