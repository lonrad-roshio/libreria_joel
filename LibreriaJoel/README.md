# Librería JOEL — Sistema de Ventas e Inventario

Proyecto Razor Pages + C# (.NET 8) + MySQL, desarrollado para el trabajo práctico
"Aplicación de Clean Code y Principios SOLID en un Proyecto de Software".

## 1. Tablas trabajadas (restricción: solo 3 tablas)

- **Cliente**: IdCliente (clave), Nombre (requerido), Direccion, Celular, NitCi (no requeridos).
- **Producto**: IdProducto (clave), Nombre, Marca, CostoEntrada, PrecioVenta, FechaIngreso,
  Stock (requeridos), Lote, UnidadMedida, Estado (no requeridos).
- **Venta**: IdVenta (clave), Fecha, IdCliente (FK), Total, FormaPago, Cajero, y **DetalleJson**:
  una columna `JSON` que guarda el arreglo de productos/cantidades vendidos
  (`IdProducto`, `NombreProducto`, `Cantidad`, `PrecioUnitario`). Se usó JSON —en vez de una
  cuarta tabla `DetalleVenta`— porque la consigna solo permite 3 tablas.

Estas 3 tablas siguen exactamente los campos definidos en las tarjetas de diseño
(imagen de Cliente/Producto/Venta) que ya habían levantado con el dueño.

## 2. Cómo se resolvieron los requerimientos de la entrevista

| Requerimiento del dueño | Dónde se resuelve |
|---|---|
| "Pierdo el control del inventario" | Módulo Productos + alertas de stock bajo en el Home (`ParametrosNegocio:StockMinimoAlerta` en `appsettings.json`) |
| Vende por paquete y también suelto (hojas, colores) | Campo `UnidadMedida` en Producto y `Stock` en unidades base (decimal) |
| Reportes de ventas en Excel | Botón "Exportar Excel" en `/Ventas` (usa ClosedXML, `VentaService.GenerarReporteExcelAsync`) |
| No se emiten facturas, solo recibo interno | `Venta` no tiene campos de facturación, solo recibo interno |
| Pago en efectivo o QR | Campo `FormaPago` en Venta |
| Varios cajeros, arqueo manual | Campo `Cajero` en cada venta (no se pidió login, según la entrevista) |
| Paleta de colores y logotipo | `wwwroot/css/site.css` (Azul Profesional #001F5B, Azul Secundario #003876,
  Arena #C5AC8F, Blanco #FFFFFF) y `wwwroot/images/logo.svg` |

## 3. Arquitectura (Clean Code + SOLID)

```
Models/         -> Entidades EF Core (solo datos + validaciones de formato)
Data/           -> DbContext
Repositories/   -> Acceso a datos, cada uno detrás de una interfaz (Inversión de Dependencias)
Services/       -> Reglas de negocio y validaciones (Responsabilidad Única)
Pages/          -> Razor Pages: solo orquestan, llaman a los Services
```

- **S**: cada clase tiene una sola razón de cambio (repositorio = datos, servicio = reglas, página = UI).
- **O**: los servicios están abiertos a extensión (nuevas reglas) sin modificar los repositorios.
- **L**: las implementaciones de `IClienteRepository`, `IProductoRepository`, `IVentaRepository`
  son sustituibles sin romper los servicios.
- **I**: interfaces específicas por entidad, no una interfaz genérica gigante.
- **D**: `Program.cs` inyecta las implementaciones; las Razor Pages y los servicios dependen
  de las interfaces, nunca de EF Core directamente.

## 4. Cómo ejecutar el proyecto

> Requiere el SDK de .NET 8 y un servidor MySQL (por ejemplo, el que trae XAMPP/WAMP o MySQL
> Server + Workbench).

1. Ejecuta `Database/libreria_joel.sql` en MySQL Workbench (crea la base `libreria_joel`,
   las 3 tablas y datos de ejemplo).
2. Ajusta la cadena de conexión en `appsettings.json` (`ConnectionStrings:LibreriaJoelConnection`)
   con tu usuario/contraseña de MySQL.
3. Desde la carpeta del proyecto:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
4. Abre `http://localhost:5210` en el navegador.

Como las tablas ya fueron creadas por el script SQL, **no** es necesario ejecutar
`dotnet ef database update`; el `DbContext` solo mapea sobre las tablas existentes.

## 5. Validaciones incluidas (18% de la rúbrica)

- Cliente: nombre obligatorio (mínimo 3 caracteres), celular solo numérico.
- Producto: nombre/marca obligatorios, costo y precio > 0, precio de venta no puede ser
  menor al costo de entrada, no se permite duplicar nombre+marca, stock no negativo.
- Venta: cliente válido y existente, al menos un producto en el detalle, cantidad > 0,
  y **no se permite vender más de lo que hay en stock** (se valida contra el inventario
  en tiempo real). Al eliminar o editar una venta, el stock se restituye automáticamente.

## 6. Notas

- No se generó output compilado (`bin/`, `obj/`) ni se ejecutó `dotnet restore` en este
  entorno porque no tiene acceso a NuGet; el proyecto se entrega como código fuente listo
  para compilar en tu máquina/laboratorio.
- El logo SVG es una interpretación simplificada de la paleta e isotipo que compartieron
  (lápiz azul con el texto "Librería JOEL").
