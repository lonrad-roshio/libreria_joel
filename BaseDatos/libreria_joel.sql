-- Libreria JOEL: base de datos relacional compatible con la aplicacion.
-- Crea usuario, producto, venta y detalle_venta (sin columnas JSON).
-- IMPORTANTE: elimina las tablas existentes del mismo nombre antes de crearlas.
-- Haz una copia de seguridad si ya tienes informacion que quieras conservar.

CREATE DATABASE IF NOT EXISTS libreria_joel
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE libreria_joel;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS detalle_venta;
DROP TABLE IF EXISTS venta;
DROP TABLE IF EXISTS producto;
DROP TABLE IF EXISTS usuario;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE usuario (
    IdUsuario INT NOT NULL AUTO_INCREMENT,
    NombreUsuario VARCHAR(50) NOT NULL,
    Nombre VARCHAR(150) NOT NULL,
    Direccion VARCHAR(250) NULL,
    Celular VARCHAR(20) NULL,
    NitCi VARCHAR(20) NULL,
    Contrasena VARCHAR(255) NOT NULL DEFAULT '',
    Rol VARCHAR(20) NOT NULL DEFAULT 'CAJERO',
    PRIMARY KEY (IdUsuario),
    UNIQUE INDEX uq_usuario_nombre_usuario (NombreUsuario)
) ENGINE = InnoDB;

CREATE TABLE producto (
    IdProducto INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(150) NOT NULL,
    Lote VARCHAR(50) NULL,
    Marca VARCHAR(80) NOT NULL,
    CostoEntrada DECIMAL(10,2) NOT NULL,
    PrecioVenta DECIMAL(10,2) NOT NULL,
    FechaIngreso DATE NOT NULL,
    Stock DECIMAL(10,2) NOT NULL DEFAULT 0,
    UnidadMedida VARCHAR(20) NOT NULL DEFAULT 'UNIDAD',
    Estado TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (IdProducto),
    INDEX idx_producto_nombre (Nombre),
    INDEX idx_producto_marca (Marca)
) ENGINE = InnoDB;

CREATE TABLE venta (
    IdVenta INT NOT NULL AUTO_INCREMENT,
    Fecha DATE NOT NULL,
    IdUsuario INT NOT NULL,
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    FormaPago VARCHAR(20) NOT NULL DEFAULT 'EFECTIVO',
    PRIMARY KEY (IdVenta),
    INDEX idx_venta_fecha (Fecha),
    INDEX idx_venta_usuario (IdUsuario),
    CONSTRAINT fk_venta_usuario
        FOREIGN KEY (IdUsuario) REFERENCES usuario (IdUsuario)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
) ENGINE = InnoDB;

CREATE TABLE detalle_venta (
    IdDetalle INT NOT NULL AUTO_INCREMENT,
    IdVenta INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad DECIMAL(10,2) NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (IdDetalle),
    INDEX idx_detalle_venta (IdVenta),
    INDEX idx_detalle_producto (IdProducto),
    CONSTRAINT fk_detalle_venta
        FOREIGN KEY (IdVenta) REFERENCES venta (IdVenta)
        ON UPDATE CASCADE
        ON DELETE CASCADE,
    CONSTRAINT fk_detalle_producto
        FOREIGN KEY (IdProducto) REFERENCES producto (IdProducto)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
) ENGINE = InnoDB;

-- 50 usuarios de ejemplo. Las contrasenas vacias no permiten iniciar sesion.
-- Al arrancar la aplicacion, configura Login__Username y Login__Password:
-- se creara una cuenta ADMIN con la contrasena guardada como hash seguro.
-- El administrador puede asignar contrasenas a los usuarios desde /Usuarios.
INSERT INTO usuario
    (NombreUsuario, Nombre, Direccion, Celular, NitCi, Contrasena, Rol)
VALUES
('cajero01', 'María Fernández Rojas', 'Av. Heroínas 245, Cochabamba', '70112233', '5487621', '', 'CAJERO'),
('cajero02', 'Juan Carlos Pérez', 'Calle Ayacucho 512, Cochabamba', '77890123', '6215489', '', 'CAJERO'),
('cajero03', 'Colegio San Agustín', 'Av. América 1200, Cochabamba', '44567891', '1023456011', '', 'CAJERO'),
('cajero04', 'Pedro Quispe Mamani', 'Villa Sebastián Pagador, Cochabamba', '69541278', '4789213', '', 'CAJERO'),
('cajero05', 'Librería Central SRL', 'Av. Blanco Galindo km 4, Cochabamba', '44223344', '1045789023', '', 'CAJERO'),
('cajero06', 'Ana María López', 'Calle España 180, Cochabamba', '70334455', '6789012', '', 'CAJERO'),
('cajero07', 'Carlos Eduardo Vargas', 'Av. Petrolera 830, Cochabamba', '75981234', '5234678', '', 'CAJERO'),
('cajero08', 'Unidad Educativa Bolívar', 'Calle Junín 740, Cochabamba', '42567890', '1024567890', '', 'CAJERO'),
('cajero09', 'Rosa Elena Gutiérrez', 'Zona Cala Cala, Cochabamba', '71456789', '4012876', '', 'CAJERO'),
('cajero10', 'Miguel Ángel Soria', 'Av. Beijing 1450, Cochabamba', '72233445', '6389012', '', 'CAJERO'),
('cajero11', 'Patricia Salazar Ríos', 'Calle Lanza 320, Cochabamba', '76432109', '5901234', '', 'CAJERO'),
('cajero12', 'Colegio 12 de Octubre', 'Av. Siglo XX 650, Cochabamba', '44321098', '1026789012', '', 'CAJERO'),
('cajero13', 'Luis Fernando Arce', 'Zona Jaihuayco, Cochabamba', '71765432', '4567890', '', 'CAJERO'),
('cajero14', 'Gabriela Rocha Molina', 'Av. Melchor Pérez 980, Cochabamba', '77901234', '6123457', '', 'CAJERO'),
('cajero15', 'Papelería El Estudiante', 'Calle Baptista 415, Cochabamba', '42654321', '1041234567', '', 'CAJERO'),
('cajero16', 'José Manuel Flores', 'Zona Tiquipaya, Cochabamba', '70678912', '5346789', '', 'CAJERO'),
('cajero17', 'Daniela Cabrera Pinto', 'Av. América Oeste 710, Cochabamba', '75210987', '6234501', '', 'CAJERO'),
('cajero18', 'Unidad Educativa San Martín', 'Calle Colombia 130, Cochabamba', '44234567', '1028901234', '', 'CAJERO'),
('cajero19', 'Verónica Méndez Alba', 'Zona Queru Queru, Cochabamba', '71987654', '4876501', '', 'CAJERO'),
('cajero20', 'Roberto Lima Rocha', 'Av. Circunvalación 220, Cochabamba', '76543210', '5987612', '', 'CAJERO'),
('cajero21', 'Mónica Andrea Cruz', 'Calle 25 de Mayo 560, Cochabamba', '70012345', '6345012', '', 'CAJERO'),
('cajero22', 'Instituto Técnico Central', 'Av. Ayacucho 900, Cochabamba', '44789012', '1023456789', '', 'CAJERO'),
('cajero23', 'Fernando Rojas Salinas', 'Zona Sarco, Cochabamba', '71234567', '5123409', '', 'CAJERO'),
('cajero24', 'Claudia Paredes Vega', 'Calle Baptista 1020, Cochabamba', '78901234', '6237890', '', 'CAJERO'),
('cajero25', 'Comercial La Escolar', 'Av. Aroma 350, Cochabamba', '42345678', '1048901234', '', 'CAJERO'),
('cajero26', 'Raúl Gonzales Tórrez', 'Zona Alalay, Cochabamba', '72109876', '4561209', '', 'CAJERO'),
('cajero27', 'Silvia Patricia León', 'Calle Sucre 275, Cochabamba', '75432198', '5789023', '', 'CAJERO'),
('cajero28', 'Colegio Nacional Sucre', 'Av. Oquendo 1510, Cochabamba', '44501234', '1025678901', '', 'CAJERO'),
('cajero29', 'Marco Antonio Tapia', 'Zona Muyurina, Cochabamba', '70876543', '6120987', '', 'CAJERO'),
('cajero30', 'Natalia Jiménez Prado', 'Calle Potosí 630, Cochabamba', '77654321', '4901238', '', 'CAJERO'),
('cajero31', 'Distribuidora Escolar Andina', 'Av. Petrolera 2040, Cochabamba', '44210987', '1045678901', '', 'CAJERO'),
('cajero32', 'Héctor Salinas Mercado', 'Zona Queru Queru, Cochabamba', '73098765', '5678901', '', 'CAJERO'),
('cajero33', 'Paola Andrea Ríos', 'Calle Tarija 445, Cochabamba', '76123450', '6781234', '', 'CAJERO'),
('cajero34', 'Unidad Educativa América', 'Av. América 1750, Cochabamba', '44345678', '1027890123', '', 'CAJERO'),
('cajero35', 'Edwin Mamani Rocha', 'Zona Valle Hermoso, Cochabamba', '71567890', '4234567', '', 'CAJERO'),
('cajero36', 'Lorena Vargas Céspedes', 'Calle Jordán 805, Cochabamba', '78234567', '5890123', '', 'CAJERO'),
('cajero37', 'Papelería Mundo Escolar', 'Av. República 915, Cochabamba', '42456789', '1046789012', '', 'CAJERO'),
('cajero38', 'Sergio Alberto Pinto', 'Zona Chimba, Cochabamba', '70789012', '6127890', '', 'CAJERO'),
('cajero39', 'Andrea Molina Céspedes', 'Calle Antezana 355, Cochabamba', '75321098', '4987650', '', 'CAJERO'),
('cajero40', 'Colegio Técnico Industrial', 'Av. Petrolera 1230, Cochabamba', '44678901', '1029012345', '', 'CAJERO'),
('cajero41', 'Óscar Beltrán Rojas', 'Zona Pacata, Cochabamba', '72456789', '5671234', '', 'CAJERO'),
('cajero42', 'Beatriz Flores Camacho', 'Calle Hamiraya 600, Cochabamba', '76890123', '6347890', '', 'CAJERO'),
('cajero43', 'Comercial Papel y Color', 'Av. Blanco Galindo 1800, Cochabamba', '44123456', '1047890123', '', 'CAJERO'),
('cajero44', 'Raquel Torrico Vela', 'Zona Villa México, Cochabamba', '71122334', '4567891', '', 'CAJERO'),
('cajero45', 'Diego Fernando Suárez', 'Calle Tumusla 725, Cochabamba', '78098765', '6234560', '', 'CAJERO'),
('cajero46', 'Unidad Educativa Loyola', 'Av. Libertador 700, Cochabamba', '44765432', '1021234567', '', 'CAJERO'),
('cajero47', 'Cecilia Aramayo López', 'Zona Temporal, Cochabamba', '71876543', '5341209', '', 'CAJERO'),
('cajero48', 'Pablo Andrés Rivero', 'Calle Nataniel Aguirre 410, Cochabamba', '75678901', '6782345', '', 'CAJERO'),
('cajero49', 'Librería y Papelería Ideas', 'Av. Santa Cruz 990, Cochabamba', '42234567', '1049012345', '', 'CAJERO'),
('cajero50', 'Teresa Camacho Vargas', 'Zona Queru Queru, Cochabamba', '72987654', '4905678', '', 'CAJERO');

-- 50 productos de ejemplo.
INSERT INTO producto
    (Nombre, Lote, Marca, CostoEntrada, PrecioVenta, FechaIngreso, Stock, UnidadMedida, Estado)
VALUES
('Bolígrafo azul punta fina', 'BOL-2601', 'BIC', 1.20, 2.50, '2026-08-01', 250, 'UNIDAD', 1),
('Bolígrafo negro punta fina', 'BOL-2602', 'BIC', 1.20, 2.50, '2026-08-01', 180, 'UNIDAD', 1),
('Caja de colores x12', 'COL-2601', 'Faber-Castell', 12.00, 22.00, '2026-08-05', 40, 'CAJA', 1),
('Caja de colores x24', 'COL-2602', 'Faber-Castell', 22.00, 38.00, '2026-08-05', 18, 'CAJA', 1),
('Caja de colores x12', 'COL-2603', 'Mon Ami', 9.50, 18.00, '2026-08-05', 30, 'CAJA', 1),
('Cuaderno universitario 100 hojas', 'CUA-2601', 'Norma', 8.00, 13.50, '2026-08-10', 60, 'UNIDAD', 1),
('Cuaderno de dibujo', 'CUA-2602', 'Norma', 9.00, 15.00, '2026-08-10', 35, 'UNIDAD', 1),
('Borrador blanco', 'BOR-2601', 'Faber-Castell', 0.80, 1.50, '2026-08-12', 200, 'UNIDAD', 1),
('Borrador blanco', 'BOR-2602', 'Pelikan', 0.70, 1.30, '2026-08-12', 4, 'UNIDAD', 1),
('Paquete de hojas tamaño carta', 'PAP-2601', 'Report', 28.00, 42.00, '2026-08-14', 25, 'PAQUETE', 1),
('Hoja suelta tamaño carta', 'PAP-2602', 'Report', 0.06, 0.10, '2026-08-14', 3500, 'HOJA', 1),
('Cartulina de colores', 'CAR-2601', 'Nacional', 1.50, 3.00, '2026-08-15', 90, 'PLIEGO', 1),
('Caja de lápices x6', 'LAP-2601', 'Faber-Castell', 6.00, 11.00, '2026-08-16', 3, 'CAJA', 1),
('Marcador permanente', 'MAR-2601', 'Artel', 3.50, 6.00, '2026-08-18', 70, 'UNIDAD', 1),
('Plastoformo A4', 'PLA-2601', 'Nacional', 4.00, 7.00, '2026-08-19', 45, 'UNIDAD', 1),
('Lápiz grafito HB', 'LAP-2602', 'Staedtler', 1.00, 2.00, '2026-08-20', 120, 'UNIDAD', 1),
('Regla plástica 30 cm', 'REG-2601', 'Maped', 2.00, 4.00, '2026-08-20', 55, 'UNIDAD', 1),
('Tijera escolar punta roma', 'TIJ-2601', 'Maped', 5.00, 9.00, '2026-08-21', 32, 'UNIDAD', 1),
('Pegamento en barra pequeño', 'PEG-2601', 'UHU', 3.00, 5.50, '2026-08-21', 48, 'UNIDAD', 1),
('Pegamento líquido 100 ml', 'PEG-2602', 'Pritt', 4.50, 8.00, '2026-08-22', 26, 'UNIDAD', 1),
('Archivador tamaño oficio', 'ARC-2601', 'Artesco', 8.00, 14.00, '2026-08-22', 20, 'UNIDAD', 1),
('Carpeta plástica con elástico', 'CAR-2602', 'Nacional', 3.00, 6.00, '2026-08-23', 65, 'UNIDAD', 1),
('Resaltador amarillo', 'RES-2601', 'Stabilo', 4.00, 7.50, '2026-08-23', 42, 'UNIDAD', 1),
('Resaltador rosado', 'RES-2602', 'Stabilo', 4.00, 7.50, '2026-08-23', 38, 'UNIDAD', 1),
('Corrector líquido', 'COR-2601', 'Liquid Paper', 3.50, 6.50, '2026-08-24', 34, 'UNIDAD', 1),
('Sacapuntas metálico', 'SAC-2601', 'Maped', 1.80, 3.50, '2026-08-24', 80, 'UNIDAD', 1),
('Sobre manila tamaño oficio', 'SOB-2601', 'Nacional', 0.80, 1.50, '2026-08-25', 150, 'UNIDAD', 1),
('Témpera escolar set x6', 'TEM-2601', 'PintArt', 8.00, 14.00, '2026-08-25', 22, 'SET', 1),
('Pincel escolar número 4', 'PIN-2601', 'Artel', 2.00, 4.00, '2026-08-26', 40, 'UNIDAD', 1),
('Papel lustre surtido', 'PAP-2603', 'Nacional', 1.00, 2.00, '2026-08-26', 100, 'PLIEGO', 1),
('Papel crepé color rojo', 'PAP-2604', 'Nacional', 1.20, 2.50, '2026-08-27', 75, 'PLIEGO', 1),
('Silicona líquida 100 ml', 'SIL-2601', 'Pegafan', 5.00, 9.00, '2026-08-27', 28, 'UNIDAD', 1),
('Cinta adhesiva transparente', 'CIN-2601', 'Scotch', 2.50, 4.50, '2026-08-28', 50, 'UNIDAD', 1),
('Cinta masking 1 pulgada', 'CIN-2602', 'Nacional', 3.00, 5.50, '2026-08-28', 36, 'UNIDAD', 1),
('Cuaderno pequeño 50 hojas', 'CUA-2603', 'Nacional', 3.50, 6.00, '2026-08-29', 70, 'UNIDAD', 1),
('Cuaderno anillado cuadriculado', 'CUA-2604', 'Norma', 10.00, 17.00, '2026-08-29', 24, 'UNIDAD', 1),
('Block de dibujo A4', 'BLO-2601', 'Lider', 7.00, 12.00, '2026-08-30', 31, 'UNIDAD', 1),
('Papel bond tamaño oficio', 'PAP-2605', 'Report', 30.00, 45.00, '2026-08-30', 19, 'PAQUETE', 1),
('Marcador para pizarra azul', 'MAR-2602', 'Pilot', 4.00, 7.00, '2026-08-31', 27, 'UNIDAD', 1),
('Marcador para pizarra negro', 'MAR-2603', 'Pilot', 4.00, 7.00, '2026-08-31', 29, 'UNIDAD', 1),
('Goma eva tamaño carta', 'GOM-2601', 'Nacional', 1.80, 3.50, '2026-09-01', 60, 'PLIEGO', 1),
('Foami escarchado', 'FOA-2601', 'Nacional', 3.00, 5.50, '2026-09-01', 18, 'PLIEGO', 1),
('Plastilina escolar x12', 'PLA-2602', 'Milan', 6.00, 10.00, '2026-09-02', 33, 'CAJA', 1),
('Papel celofán transparente', 'PAP-2606', 'Nacional', 1.50, 3.00, '2026-09-02', 40, 'PLIEGO', 1),
('Clip metálico caja pequeña', 'CLI-2601', 'Artesco', 2.00, 3.50, '2026-09-03', 55, 'CAJA', 1),
('Grapas estándar caja', 'GRA-2601', 'Artesco', 2.50, 4.50, '2026-09-03', 44, 'CAJA', 1),
('Perforadora pequeña', 'PER-2601', 'Maped', 12.00, 20.00, '2026-09-04', 12, 'UNIDAD', 1),
('Engrapadora metálica', 'ENG-2601', 'Artesco', 15.00, 25.00, '2026-09-04', 9, 'UNIDAD', 1),
('Calculadora básica', 'CAL-2601', 'Casio', 35.00, 55.00, '2026-09-05', 14, 'UNIDAD', 1),
('Papel milimetrado', 'PAP-2607', 'Nacional', 0.80, 1.50, '2026-09-05', 85, 'HOJA', 1);

-- 50 ventas y sus 50 detalles de ejemplo (un producto por venta).
INSERT INTO venta (Fecha, IdUsuario, Total, FormaPago)
SELECT
    DATE_SUB(CURDATE(), INTERVAL (50 - usuario.IdUsuario) DAY),
    usuario.IdUsuario,
    0,
    CASE WHEN MOD(usuario.IdUsuario, 2) = 0 THEN 'QR' ELSE 'EFECTIVO' END
FROM usuario
WHERE usuario.IdUsuario BETWEEN 1 AND 50;

INSERT INTO detalle_venta (IdVenta, IdProducto, Cantidad, PrecioUnitario, Subtotal)
SELECT
    venta.IdVenta,
    producto.IdProducto,
    MOD(venta.IdUsuario, 4) + 1,
    producto.PrecioVenta,
    producto.PrecioVenta * (MOD(venta.IdUsuario, 4) + 1)
FROM venta
INNER JOIN producto ON producto.IdProducto = venta.IdUsuario
WHERE venta.IdUsuario BETWEEN 1 AND 50;

UPDATE venta
SET Total = (
    SELECT SUM(detalle_venta.Subtotal)
    FROM detalle_venta
    WHERE detalle_venta.IdVenta = venta.IdVenta
);

UPDATE producto
INNER JOIN detalle_venta ON detalle_venta.IdProducto = producto.IdProducto
SET producto.Stock = producto.Stock - detalle_venta.Cantidad;

-- Las cuatro consultas deben devolver 50.
SELECT 'usuario' AS Tabla, COUNT(*) AS Registros FROM usuario
UNION ALL
SELECT 'producto', COUNT(*) FROM producto
UNION ALL
SELECT 'venta', COUNT(*) FROM venta
UNION ALL
SELECT 'detalle_venta', COUNT(*) FROM detalle_venta;
