namespace LibreriaJoel.Models;

/// <summary>
/// Línea de detalle dentro de una venta (producto + cantidad + precio unitario).
/// NO es una tabla independiente: el trabajo práctico limita el diseño a 3 tablas
/// (Cliente, Producto, Venta), por lo que la lista de productos vendidos se serializa
/// como JSON dentro de la columna Venta.DetalleJson.
/// </summary>
public class DetalleVentaItem
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Math.Round(Cantidad * PrecioUnitario, 2);
}
