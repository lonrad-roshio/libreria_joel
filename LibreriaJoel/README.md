# Librería JOEL — Sistema de ventas e inventario

Aplicación Razor Pages con C# (.NET 8) y MySQL.

## Esquema de base de datos

El proyecto usa cuatro tablas relacionales, sin guardar el detalle de ventas como JSON:

- **usuario**: nombre de usuario, nombre, dirección, celular, NIT/CI, contraseña con hash
  seguro y rol (`ADMIN` o `CAJERO`).
- **producto**: nombre, lote, marca, costo, precio, fecha de ingreso, stock, unidad de medida
  y estado.
- **venta**: fecha, usuario que registró la venta, total y forma de pago.
- **detalle_venta**: producto, cantidad, precio unitario y subtotal; enlaza ventas con productos.

## Preparación y ejecución

1. En MySQL Workbench ejecuta `../BaseDatos/libreria_joel.sql`. El script crea las cuatro
   tablas y agrega 50 ejemplos de usuarios, productos, ventas y detalles.
   **Elimina y recrea las tablas con esos nombres**; respalda tus datos antes de ejecutarlo.
2. Configura `ConnectionStrings:LibreriaJoelConnection` en `appsettings.json` con la conexión
   de tu servidor MySQL.
3. Antes del primer inicio, define `Login__Username` y `Login__Password` en el entorno de
   ejecución. La contraseña debe tener al menos 12 caracteres. Si no existe un administrador
   con hash de contraseña, la aplicación crea uno usando esas variables. No guardes la
   contraseña en el repositorio ni en `appsettings.json`.
4. Desde la carpeta del proyecto ejecuta:

   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

5. Inicia sesión en `/Account/Login`. La gestión de usuarios está en `/Usuarios` y requiere
   el rol `ADMIN`. Las cuentas de ejemplo tienen la contraseña vacía hasta que un administrador
   les asigne una contraseña desde la aplicación.

Como el esquema lo crea el script SQL, no se requiere ejecutar `dotnet ef database update`.

## Reglas principales

- Las contraseñas se guardan con `PasswordHasher` de ASP.NET Core, nunca como texto plano.
- Se permite un detalle de venta con varias líneas; MySQL conserva cada línea en `detalle_venta`.
- Al registrar una venta se comprueba el stock y se descuenta; al editar o eliminar se ajusta
  el inventario correspondiente.
- Los roles limitan la gestión de cuentas al administrador.
- La aplicación acepta pagos en efectivo y QR, y genera reportes de ventas en Excel.
- Usa HTTPS. Las cookies de sesión son `HttpOnly`, `Secure` y `SameSite=Strict`.
