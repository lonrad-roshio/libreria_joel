using LibreriaJoel.Models;

namespace LibreriaJoel.Repositories.Interfaces;

public interface IVentaRepository : IRepository<Venta>
{
    Task<List<Venta>> ListarAsync(DateTime? fecha = null);
    Task<List<Venta>> ObtenerEntreFechasAsync(DateTime desde, DateTime hasta);
    Task<decimal> ObtenerIngresosDelDiaAsync(DateTime fecha);
}
