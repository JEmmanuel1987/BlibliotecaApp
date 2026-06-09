
CREATE DATABASE BibliotecaDB;
GO

USE BibliotecaDB;
GO

-- Tabla: Categorias
CREATE TABLE Categorias (
    CategoriaId   INT IDENTITY(1,1) PRIMARY KEY,
    Nombre        NVARCHAR(100) NOT NULL,
    Descripcion   NVARCHAR(MAX) NULL
);

-- Tabla: Libros
CREATE TABLE Libros (
    LibroId          INT IDENTITY(1,1) PRIMARY KEY,
    ISBN             NVARCHAR(20)  NOT NULL UNIQUE,
    Titulo           NVARCHAR(200) NOT NULL,
    Autor            NVARCHAR(150) NOT NULL,
    AnioPublicacion  INT           NOT NULL,
    Editorial        NVARCHAR(100) NOT NULL DEFAULT '',
    StockTotal       INT           NOT NULL DEFAULT 1,
    StockDisponible  INT           NOT NULL DEFAULT 1,
    Activo           BIT           NOT NULL DEFAULT 1,
    CategoriaId      INT           NOT NULL,
    CONSTRAINT FK_Libros_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(CategoriaId)
);

-- Tabla: Socios
CREATE TABLE Socios (
    SocioId         INT IDENTITY(1,1) PRIMARY KEY,
    NroCarnet       NVARCHAR(20)  NOT NULL UNIQUE,
    Nombre          NVARCHAR(100) NOT NULL,
    Apellido        NVARCHAR(100) NOT NULL,
    Telefono        NVARCHAR(50)  NULL,
    Email           NVARCHAR(150) NULL,
    FechaRegistro   DATETIME      NOT NULL DEFAULT GETDATE(),
    Activo          BIT           NOT NULL DEFAULT 1,
    DeudaTotal      DECIMAL(10,2) NOT NULL DEFAULT 0
);

-- Tabla: Prestamos
CREATE TABLE Prestamos (
    PrestamoId              INT IDENTITY(1,1) PRIMARY KEY,
    FechaPrestamo           DATETIME      NOT NULL DEFAULT GETDATE(),
    FechaDevolucionEsperada DATETIME      NOT NULL,
    FechaDevolucionReal     DATETIME      NULL,
    Estado                  NVARCHAR(20)  NOT NULL DEFAULT 'Activo',  -- Activo | Devuelto | Vencido
    MultaGenerada           DECIMAL(10,2) NOT NULL DEFAULT 0,
    MultaPagada             BIT           NOT NULL DEFAULT 0,
    Observaciones           NVARCHAR(MAX) NULL,
    LibroId                 INT           NOT NULL,
    SocioId                 INT           NOT NULL,
    CONSTRAINT FK_Prestamos_Libros  FOREIGN KEY (LibroId)  REFERENCES Libros(LibroId),
    CONSTRAINT FK_Prestamos_Socios  FOREIGN KEY (SocioId)  REFERENCES Socios(SocioId)
);

-- ── Datos semilla ─────────────────────────────────────────────────────────────
INSERT INTO Categorias (Nombre, Descripcion) VALUES
    ('Ciencias Exactas',    'Matemáticas, Física, Química'),
    ('Ingeniería',           'Sistemas, Civil, Industrial'),
    ('Literatura',           'Novela, Cuento, Poesía'),
    ('Historia y Geografía', 'Historia universal y boliviana'),
    ('Derecho',              'Derecho civil, penal, comercial');

-- Libros de ejemplo
INSERT INTO Libros (ISBN, Titulo, Autor, AnioPublicacion, Editorial, StockTotal, StockDisponible, CategoriaId) VALUES
    ('978-607-07-7498-0', 'Cálculo Diferencial e Integral', 'James Stewart',      2020, 'Cengage',   3, 3, 1),
    ('978-970-26-1641-2', 'Álgebra Lineal',                 'Howard Anton',       2019, 'Wiley',     2, 2, 1),
    ('978-607-32-4754-1', 'Ingeniería de Software',         'Roger Pressman',     2021, 'McGraw',    4, 4, 2),
    ('978-84-9835-063-7', 'Clean Code',                     'Robert C. Martin',   2018, 'Prentice',  2, 2, 2),
    ('978-950-07-1279-3', 'Cien años de soledad',           'Gabriel García Mrqz',1967, 'Sudamercna',3, 3, 3);
