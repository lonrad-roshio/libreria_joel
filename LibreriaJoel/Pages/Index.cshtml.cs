using LibreriaJoel.Models;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages;

public class IndexModel : PageModel
{
    private readonly IProductoService _productoService;
    private readonly IVentaService _ventaService;

    public IndexModel(IProductoService productoService, IVentaService ventaService)
    {
        _productoService = productoService;
        _ventaService = ventaService;
    }

    public List<Producto> ProductosStockBajo { get; set; } = new();
    public decimal IngresosDeHoy { get; set; }
    public int VentasDeHoy { get; set; }

    public async Task OnGetAsync()
    {
        ProductosStockBajo = await _productoService.ObtenerAlertasStockBajoAsync();
        IngresosDeHoy = await _ventaService.ObtenerIngresosDelDiaAsync(DateTime.Today);
        VentasDeHoy = (await _ventaService.ListarAsync(DateTime.Today)).Count;
    }
}
