-- =====================================================================
-- Librería JOEL - Sistema de Ventas e Inventario
-- Script de base de datos para MySQL Workbench
-- Contiene: creación de base de datos, 3 tablas (Cliente, Producto,
-- Venta - según restricción del trabajo práctico), datos de ejemplo
-- (llenados a partir de la información de las entrevistas) y algunas
-- consultas de referencia (reportes) pedidas en el enunciado.
-- =====================================================================

DROP DATABASE IF EXISTS libreria_joel;
CREATE DATABASE libreria_joel CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE libreria_joel;

-- ---------------------------------------------------------------------
-- Tabla: cliente
-- ---------------------------------------------------------------------
CREATE TABLE cliente (
    IdCliente   INT AUTO_INCREMENT PRIMARY KEY,
    Nombre      VARCHAR(150) NOT NULL,
    Direccion   VARCHAR(250) NULL,
    Celular     VARCHAR(20)  NULL,
    NitCi       VARCHAR(20)  NULL
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Tabla: producto
-- El stock se maneja en "unidades base" para poder registrar tanto
-- ventas por paquete/caja como ventas de unidades sueltas (hojas,
-- colores, etc.), tal como lo explicó el dueño en la entrevista.
-- ---------------------------------------------------------------------
CREATE TABLE producto (
    IdProducto    INT AUTO_INCREMENT PRIMARY KEY,
    Nombre        VARCHAR(150) NOT NULL,
    Lote          VARCHAR(50)  NULL,
    Marca         VARCHAR(80)  NOT NULL,
    CostoEntrada  DECIMAL(10,2) NOT NULL,
    PrecioVenta   DECIMAL(10,2) NOT NULL,
    FechaIngreso  DATE NOT NULL,
    Stock         DECIMAL(10,2) NOT NULL DEFAULT 0,
    UnidadMedida  VARCHAR(20) NOT NULL DEFAULT 'UNIDAD',
    Estado        TINYINT(1) NOT NULL DEFAULT 1,
    INDEX idx_producto_nombre (Nombre)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Tabla: venta
-- DetalleJson guarda el arreglo de productos/cantidades vendidos
-- (IdProducto, NombreProducto, Cantidad, PrecioUnitario) porque el
-- trabajo práctico solo permite 3 tablas (no se puede crear una tabla
-- independiente de "detalle_venta").
-- ---------------------------------------------------------------------
CREATE TABLE venta (
    IdVenta     INT AUTO_INCREMENT PRIMARY KEY,
    Fecha       DATE NOT NULL,
    IdCliente   INT NOT NULL,
    DetalleJson JSON NOT NULL,
    Total       DECIMAL(10,2) NOT NULL,
    FormaPago   VARCHAR(20) NOT NULL DEFAULT 'EFECTIVO',
    Cajero      VARCHAR(100) NOT NULL,
    CONSTRAINT fk_venta_cliente FOREIGN KEY (IdCliente) REFERENCES cliente(IdCliente)
) ENGINE=InnoDB;

-- =====================================================================
-- DATOS DE EJEMPLO
-- =====================================================================

INSERT INTO cliente (Nombre, Direccion, Celular, NitCi) VALUES
('Cliente Ocasional', NULL, NULL, NULL),
('María Fernández Rojas', 'Av. Heroínas #245, Cochabamba', '70112233', '5487621'),
('Juan Carlos Pérez', 'Calle Ayacucho #512', '77890123', '6215489'),
('Colegio San Agustín', 'Av. América #1200', '4-4456789', '1023456011'),
('Pedro Quispe Mamani', 'Zona Sur, Villa Sebastián Pagador', '69541278', '4789213'),
('Librería Central S.R.L.', 'Av. Blanco Galindo Km 4', '4-4223344', '1045789023');

INSERT INTO producto (Nombre, Lote, Marca, CostoEntrada, PrecioVenta, FechaIngreso, Stock, UnidadMedida, Estado) VALUES
('Bolígrafo azul punta fina', 'L-2026-01', 'BIC', 1.20, 2.50, '2026-08-01', 250, 'UNIDAD', 1),
('Bolígrafo negro punta fina', 'L-2026-01', 'BIC', 1.20, 2.50, '2026-08-01', 180, 'UNIDAD', 1),
('Caja de colores x12', 'L-2026-03', 'Faber-Castell', 12.00, 22.00, '2026-08-05', 40, 'CAJA', 1),
('Caja de colores x24', 'L-2026-03', 'Faber-Castell', 22.00, 38.00, '2026-08-05', 18, 'CAJA', 1),
('Caja de colores x12', 'L-2026-04', 'Mon Ami', 9.50, 18.00, '2026-08-05', 30, 'CAJA', 1),
('Cuaderno universitario 100 hojas', 'L-2026-02', 'Norma', 8.00, 13.50, '2026-08-10', 60, 'UNIDAD', 1),
('Cuaderno de dibujo', 'L-2026-02', 'Norma', 9.00, 15.00, '2026-08-10', 35, 'UNIDAD', 1),
('Borrador blanco', 'L-2026-05', 'Faber-Castell', 0.80, 1.50, '2026-08-12', 200, 'UNIDAD', 1),
('Borrador blanco', 'L-2026-05', 'Pelikan', 0.70, 1.30, '2026-08-12', 4, 'UNIDAD', 1),
('Paquete de hojas tamaño carta (500u)', 'L-2026-06', 'Report', 28.00, 42.00, '2026-08-14', 25, 'PAQUETE', 1),
('Hoja suelta tamaño carta', 'L-2026-06', 'Report', 0.06, 0.10, '2026-08-14', 3500, 'HOJA', 1),
('Cartulina de colores (pliego)', 'L-2026-07', 'Nacional', 1.50, 3.00, '2026-08-15', 90, 'UNIDAD', 1),
('Caja de lápices x6', 'L-2026-08', 'Faber-Castell', 6.00, 11.00, '2026-08-16', 3, 'CAJA', 1),
('Marcador permanente', 'L-2026-09', 'Artel', 3.50, 6.00, '2026-08-18', 70, 'UNIDAD', 1),
('Plastoformo A4', 'L-2026-10', 'Nacional', 4.00, 7.00, '2026-08-19', 45, 'UNIDAD', 1);

-- Ventas de ejemplo (el detalle se guarda como JSON dentro de cada fila)
INSERT INTO venta (Fecha, IdCliente, DetalleJson, Total, FormaPago, Cajero) VALUES
('2026-09-20', 2,
 JSON_ARRAY(
   JSON_OBJECT('idProducto', 1, 'nombreProducto', 'Bolígrafo azul punta fina', 'cantidad', 3, 'precioUnitario', 2.50),
   JSON_OBJECT('idProducto', 6, 'nombreProducto', 'Cuaderno universitario 100 hojas', 'cantidad', 2, 'precioUnitario', 13.50)
 ), 34.50, 'EFECTIVO', 'Ana Torrez'),
('2026-09-21', 4,
 JSON_ARRAY(
   JSON_OBJECT('idProducto', 3, 'nombreProducto', 'Caja de colores x12', 'cantidad', 10, 'precioUnitario', 22.00),
   JSON_OBJECT('idProducto', 10, 'nombreProducto', 'Paquete de hojas tamaño carta (500u)', 'cantidad', 5, 'precioUnitario', 42.00)
 ), 430.00, 'QR', 'Ana Torrez'),
('2026-09-22', 1,
 JSON_ARRAY(
   JSON_OBJECT('idProducto', 11, 'nombreProducto', 'Hoja suelta tamaño carta', 'cantidad', 20, 'precioUnitario', 0.10)
 ), 2.00, 'EFECTIVO', 'Luis Choque'),
('2026-09-22', 3,
 JSON_ARRAY(
   JSON_OBJECT('idProducto', 8, 'nombreProducto', 'Borrador blanco', 'cantidad', 4, 'precioUnitario', 1.50),
   JSON_OBJECT('idProducto', 14, 'nombreProducto', 'Marcador permanente', 'cantidad', 2, 'precioUnitario', 6.00)
 ), 18.00, 'QR', 'Luis Choque'),
('2026-09-23', 6,
 JSON_ARRAY(
   JSON_OBJECT('idProducto', 4, 'nombreProducto', 'Caja de colores x24', 'cantidad', 6, 'precioUnitario', 38.00),
   JSON_OBJECT('idProducto', 12, 'nombreProducto', 'Cartulina de colores (pliego)', 'cantidad', 15, 'precioUnitario', 3.00)
 ), 273.00, 'EFECTIVO', 'Ana Torrez');

-- Descontar del stock lo ya vendido en los datos de ejemplo, para que el
-- inventario mostrado en el sistema sea consistente con las ventas de arriba.
UPDATE producto SET Stock = Stock - 3  WHERE IdProducto = 1;
UPDATE producto SET Stock = Stock - 2  WHERE IdProducto = 6;
UPDATE producto SET Stock = Stock - 10 WHERE IdProducto = 3;
UPDATE producto SET Stock = Stock - 5  WHERE IdProducto = 10;
UPDATE producto SET Stock = Stock - 20 WHERE IdProducto = 11;
UPDATE producto SET Stock = Stock - 4  WHERE IdProducto = 8;
UPDATE producto SET Stock = Stock - 2  WHERE IdProducto = 14;
UPDATE producto SET Stock = Stock - 6  WHERE IdProducto = 4;
UPDATE producto SET Stock = Stock - 15 WHERE IdProducto = 12;

-- =====================================================================
-- CONSULTAS DE REFERENCIA (reportes solicitados en la entrevista y en
-- el enunciado del trabajo: listado ordenado con "refresco" de stock,
-- alertas de stock bajo y reporte de ventas)
-- =====================================================================

-- 1) Listado de productos ordenado por nombre, con su stock actual (Select con datos ordenados)
SELECT IdProducto, Nombre, Marca, Stock, UnidadMedida, Estado
FROM producto
ORDER BY Nombre ASC;

-- 2) Alerta de stock bajo (el dueño pidió que el sistema le avise cuando le quedan pocos productos)
SELECT IdProducto, Nombre, Marca, Stock, UnidadMedida
FROM producto
WHERE Estado = 1 AND Stock <= 5
ORDER BY Stock ASC;

-- 3) Reporte de ventas por día con el total ingresado (arqueo/cierre de caja)
SELECT Fecha, Cajero, COUNT(*) AS CantidadVentas, SUM(Total) AS TotalIngresado
FROM venta
GROUP BY Fecha, Cajero
ORDER BY Fecha DESC;

-- 4) Detalle de productos vendidos por venta, extrayendo el JSON (ejemplo de consulta sobre DetalleJson)
SELECT
    v.IdVenta,
    v.Fecha,
    c.Nombre AS Cliente,
    JSON_UNQUOTE(JSON_EXTRACT(item.value, '$.nombreProducto')) AS Producto,
    JSON_EXTRACT(item.value, '$.cantidad')       AS Cantidad,
    JSON_EXTRACT(item.value, '$.precioUnitario') AS PrecioUnitario
FROM venta v
JOIN cliente c ON c.IdCliente = v.IdCliente
JOIN JSON_TABLE(
        v.DetalleJson, '$[*]'
        COLUMNS (value JSON PATH '$')
     ) AS item
ORDER BY v.Fecha DESC, v.IdVenta;

-- 5) Top clientes por monto total comprado
SELECT c.Nombre, COUNT(v.IdVenta) AS CantidadCompras, SUM(v.Total) AS TotalComprado
FROM cliente c
JOIN venta v ON v.IdCliente = c.IdCliente
GROUP BY c.IdCliente, c.Nombre
ORDER BY TotalComprado DESC;
