using LibreriaJoel.Models;

namespace LibreriaJoel.Services.Interfaces;

public interface IVentaService
{
    Task<List<Venta>> ListarAsync(DateTime? fecha = null);
    Task<Venta?> ObtenerAsync(int id);
    Task RegistrarVentaAsync(Venta venta);
    Task ActualizarAsync(Venta venta, Venta ventaOriginal);
    Task EliminarAsync(int id);
    Task<decimal> ObtenerIngresosDelDiaAsync(DateTime fecha);
    Task<byte[]> GenerarReporteExcelAsync(DateTime desde, DateTime hasta);
}
