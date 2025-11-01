-- ============================================
-- Base de Datos: ParcialZEnervn
-- Sistema de Gestión de Canal de Televisión
-- ============================================

-- Crear la base de datos
CREATE DATABASE Parcial2Jjbr;
GO

USE Parcial2Jjbr;
GO

-- ============================================
-- CREAR TABLAS
-- ============================================

-- Tabla Canal
CREATE TABLE Canal (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    frecuencia VARCHAR(20) NOT NULL,
    estado SMALLINT NOT NULL DEFAULT 1
);
GO

-- Tabla Programa
CREATE TABLE Programa (
    id INT IDENTITY(1,1) PRIMARY KEY,
    idCanal INT NOT NULL,
    titulo VARCHAR(100) NOT NULL,
    descripcion VARCHAR(250),
    duracion INT NOT NULL,
    productor VARCHAR(100),
    fechaEstreno DATE NOT NULL,
    estado SMALLINT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Programa_Canal FOREIGN KEY (idCanal) REFERENCES Canal(id)
);
GO

-- ============================================
-- CREAR USUARIO
-- ============================================

-- Crear login y usuario
USE [master];
GO
DROP LOGIN [usrparcial2];
GO

DROP USER IF EXISTS [usrparcial2];
GO

CREATE LOGIN usrparcial2 WITH PASSWORD = '12345678';
GO

CREATE USER usrparcial2 FOR LOGIN usrparcial2;
GO

-- Asignar permisos
GRANT SELECT, INSERT, UPDATE, DELETE ON Canal TO usrparcial2;
GRANT SELECT, INSERT, UPDATE, DELETE ON Programa TO usrparcial2;
GO

-- ============================================
-- PROCEDIMIENTOS ALMACENADOS - CANAL
-- ============================================

-- LISTAR Canales
CREATE PROCEDURE sp_ListarCanales
AS
BEGIN
    SELECT 
        id,
        nombre,
        frecuencia,
        estado
    FROM Canal
    WHERE estado = 1
    ORDER BY nombre;
END;
GO

-- CREAR Canal
CREATE PROCEDURE sp_CrearCanal
    @nombre VARCHAR(50),
    @frecuencia VARCHAR(20)
AS
BEGIN
    INSERT INTO Canal (nombre, frecuencia, estado)
    VALUES (@nombre, @frecuencia, 1);
    
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

-- LEER Canal por ID
CREATE PROCEDURE sp_LeerCanal
    @id INT
AS
BEGIN
    SELECT 
        id,
        nombre,
        frecuencia,
        estado
    FROM Canal
    WHERE id = @id AND estado = 1;
END;
GO

-- ACTUALIZAR Canal
CREATE PROCEDURE sp_ActualizarCanal
    @id INT,
    @nombre VARCHAR(50),
    @frecuencia VARCHAR(20)
AS
BEGIN
    UPDATE Canal
    SET nombre = @nombre,
        frecuencia = @frecuencia
    WHERE id = @id AND estado = 1;
END;
GO

-- ELIMINAR Canal (eliminación lógica)
CREATE PROCEDURE sp_EliminarCanal
    @id INT
AS
BEGIN
    UPDATE Canal
    SET estado = 0
    WHERE id = @id;
END;
GO

-- ============================================
-- PROCEDIMIENTOS ALMACENADOS - PROGRAMA
-- ============================================
create proc paProgramaListar @parametro varchar(100)
as
    select
            p.id,
        p.idCanal,
        c.nombre AS nombreCanal,
        p.titulo,
        p.descripcion,
        p.duracion,
        p.productor,
        p.fechaEstreno,
        p.estado
-- LISTAR Programas
drop procedure if exists sp_ListarProgramas;
go

CREATE PROCEDURE paProgramaListar
AS
BEGIN
    SELECT 
        p.id,
        p.idCanal,
        c.nombre AS nombreCanal,
        p.titulo,
        p.descripcion,
        p.duracion,
        p.productor,
        p.fechaEstreno,
        p.estado
    FROM Programa p
    INNER JOIN Canal c ON p.idCanal = c.id
    WHERE p.estado = 1
    ORDER BY p.fechaEstreno DESC, p.titulo;
END;
GO

-- LISTAR Programas por Canal
CREATE PROCEDURE sp_ListarProgramasPorCanal
    @idCanal INT
AS
BEGIN
    SELECT 
        p.id,
        p.idCanal,
        p.titulo,
        p.descripcion,
        p.duracion,
        p.productor,
        p.fechaEstreno,
        p.estado
    FROM Programa p
    WHERE p.idCanal = @idCanal AND p.estado = 1
    ORDER BY p.fechaEstreno DESC;
END;
GO

-- CREAR Programa
CREATE PROCEDURE sp_CrearPrograma
    @idCanal INT,
    @titulo VARCHAR(100),
    @descripcion VARCHAR(250),
    @duracion INT,
    @productor VARCHAR(100),
    @fechaEstreno DATE
AS
BEGIN
    INSERT INTO Programa (idCanal, titulo, descripcion, duracion, productor, fechaEstreno, estado)
    VALUES (@idCanal, @titulo, @descripcion, @duracion, @productor, @fechaEstreno, 1);
    
    SELECT SCOPE_IDENTITY() AS id;
END;
GO

-- LEER Programa por ID
CREATE PROCEDURE sp_LeerPrograma
    @id INT
AS
BEGIN
    SELECT 
        p.id,
        p.idCanal,
        c.nombre AS nombreCanal,
        p.titulo,
        p.descripcion,
        p.duracion,
        p.productor,
        p.fechaEstreno,
        p.estado
    FROM Programa p
    INNER JOIN Canal c ON p.idCanal = c.id
    WHERE p.id = @id AND p.estado = 1;
END;
GO

-- ACTUALIZAR Programa
CREATE PROCEDURE sp_ActualizarPrograma
    @id INT,
    @idCanal INT,
    @titulo VARCHAR(100),
    @descripcion VARCHAR(250),
    @duracion INT,
    @productor VARCHAR(100),
    @fechaEstreno DATE
AS
BEGIN
    UPDATE Programa
    SET idCanal = @idCanal,
        titulo = @titulo,
        descripcion = @descripcion,
        duracion = @duracion,
        productor = @productor,
        fechaEstreno = @fechaEstreno
    WHERE id = @id AND estado = 1;
END;
GO

-- ELIMINAR Programa (eliminación lógica)
CREATE PROCEDURE sp_EliminarPrograma
    @id INT
AS
BEGIN
    UPDATE Programa
    SET estado = 0
    WHERE id = @id;
END;
GO

-- ============================================
-- DATOS DE PRUEBA
-- ============================================

-- Insertar canales de prueba
INSERT INTO Canal (nombre, frecuencia, estado) VALUES 
('Canal Nacional', '105.5 FM', 1),
('Deportes TV', '200.1 UHF', 1),
('Noticias 24/7', '89.7 FM', 1),
('Entretenimiento Plus', '175.3 UHF', 1);
GO

-- Insertar programas de prueba
INSERT INTO Programa (idCanal, titulo, descripcion, duracion, productor, fechaEstreno, estado) VALUES 
(1, 'Noticiero Matutino', 'Noticias nacionales e internacionales', 60, 'Juan Pérez', '2025-01-15', 1),
(1, 'Cultura y Tradición', 'Programa sobre la cultura boliviana', 45, 'María González', '2025-02-01', 1),
(2, 'Fútbol en Vivo', 'Transmisión de partidos de fútbol', 120, 'Carlos Rodríguez', '2025-01-20', 1),
(2, 'Deportes Extremos', 'Lo mejor de deportes de aventura', 30, 'Ana López', '2025-03-10', 1),
(3, 'Análisis Político', 'Debate y análisis de actualidad política', 90, 'Roberto Sánchez', '2025-01-25', 1),
(4, 'Cine de Medianoche', 'Películas clásicas y contemporáneas', 150, 'Laura Martínez', '2025-02-14', 1);
GO

-- ============================================
-- CONSULTAS DE VERIFICACIÓN
-- ============================================

PRINT '=== VERIFICACIÓN DE DATOS ===';
GO

-- Listar todos los canales
PRINT 'Canales activos:';
EXEC sp_ListarCanales;
GO

-- Listar todos los programas
PRINT 'Programas activos:';
EXEC sp_ListarProgramas;
GO

PRINT 'Base de datos creada exitosamente!';