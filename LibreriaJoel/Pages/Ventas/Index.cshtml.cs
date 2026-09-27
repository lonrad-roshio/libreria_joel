using System.ComponentModel.DataAnnotations;
using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Ventas;

public class IndexModel : PageModel
{
    private readonly IVentaService _ventaService;

    public IndexModel(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    public List<Venta> Ventas { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; }

    public decimal TotalFiltrado { get; set; }

    public async Task OnGetAsync()
    {
        Ventas = await _ventaService.ListarAsync(Fecha);
        TotalFiltrado = Ventas.Sum(v => v.Total);
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
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
        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetExportarExcelAsync(DateTime? desde, DateTime? hasta)
    {
        var inicio = desde ?? DateTime.Today.AddDays(-30);
        var fin = hasta ?? DateTime.Today;
        var archivo = await _ventaService.GenerarReporteExcelAsync(inicio, fin);
        var nombre = $"reporte_ventas_{inicio:yyyyMMdd}_{fin:yyyyMMdd}.xlsx";
        return File(archivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombre);
    }
}
