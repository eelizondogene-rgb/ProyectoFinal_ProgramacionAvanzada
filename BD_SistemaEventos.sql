
-- ============================================================
-- SISTEMAEVENTOS - CREACIÓN DE BASE DE DATOS
-- ============================================================

-- Crear la base de datos únicamente si no existe
IF DB_ID(N'BD_SistemaEventos') IS NULL
BEGIN
    CREATE DATABASE [BD_SistemaEventos];
END;
GO

-- Seleccionar la base de datos para ejecutar las migraciones
USE [BD_SistemaEventos];
GO

-- ============================================================
-- MIGRACIONES DE ENTITY FRAMEWORK CORE
-- ============================================================

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261031174252_InicialUsuarios'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellido] nvarchar(100) NOT NULL,
        [Correo] nvarchar(150) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Rol] nvarchar(20) NOT NULL,
        [Activo] bit NOT NULL,
        [FechaRegistro] datetime2 NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261031174252_InicialUsuarios'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261031174252_InicialUsuarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261031174252_InicialUsuarios', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE TABLE [Eventos] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(150) NOT NULL,
        [Lugar] nvarchar(150) NOT NULL,
        [FechaEvento] datetime2 NULL,
        [Capacidad] int NOT NULL,
        [PrecioEntrada] int NOT NULL,
        [FechaCreacion] datetime2 NULL,
        CONSTRAINT [PK_Eventos] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE TABLE [Asientos] (
        [Id] int NOT NULL IDENTITY,
        [EventoId] int NOT NULL,
        [Fila] int NOT NULL,
        [Columna] int NOT NULL,
        CONSTRAINT [PK_Asientos] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Asientos_Id_EventoId] UNIQUE ([Id], [EventoId]),
        CONSTRAINT [FK_Asientos_Eventos_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [Eventos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE TABLE [Compras] (
        [Id] int NOT NULL IDENTITY,
        [UsuarioId] int NOT NULL,
        [EventoId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Total] int NOT NULL,
        CONSTRAINT [PK_Compras] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Compras_Id_EventoId] UNIQUE ([Id], [EventoId]),
        CONSTRAINT [FK_Compras_Eventos_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [Eventos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Compras_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE TABLE [Boletos] (
        [Id] int NOT NULL IDENTITY,
        [CompraId] int NOT NULL,
        [AsientoId] int NOT NULL,
        [EventoId] int NOT NULL,
        CONSTRAINT [PK_Boletos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Boletos_Asientos_AsientoId_EventoId] FOREIGN KEY ([AsientoId], [EventoId]) REFERENCES [Asientos] ([Id], [EventoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Boletos_Compras_CompraId_EventoId] FOREIGN KEY ([CompraId], [EventoId]) REFERENCES [Compras] ([Id], [EventoId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Asientos_EventoId_Fila_Columna] ON [Asientos] ([EventoId], [Fila], [Columna]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Boletos_AsientoId] ON [Boletos] ([AsientoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE INDEX [IX_Boletos_AsientoId_EventoId] ON [Boletos] ([AsientoId], [EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE INDEX [IX_Boletos_CompraId_EventoId] ON [Boletos] ([CompraId], [EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE INDEX [IX_Compras_EventoId] ON [Compras] ([EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    CREATE INDEX [IX_Compras_UsuarioId] ON [Compras] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103012126_AgregarEventosAsientosComprasBoletos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103012126_AgregarEventosAsientosComprasBoletos', N'10.0.14');
END;

COMMIT;
GO
