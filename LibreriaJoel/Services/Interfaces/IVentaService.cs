using LibreriaJoel.Models;

namespace LibreriaJoel.Services.Interfaces;

public interface IVentaService
{
    Task<List<Venta>> ListarAsync(DateTime? fecha);
    Task<Venta?> ObtenerAsync(int id);
    Task<Venta> RegistrarVentaAsync(Venta venta);
    Task ActualizarAsync(Venta ventaActualizada, Venta ventaOriginal);
    Task EliminarAsync(int id);
    Task<decimal> ObtenerIngresosDelDiaAsync(DateTime fecha);
    Task<byte[]> GenerarReporteExcelAsync(DateTime desde, DateTime hasta);
}
