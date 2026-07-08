# FASE 2: Diseño de Base de Datos y Modelo Relacional
**Proyecto:** BiblioTech - Sistema de Gestión de Biblioteca Central UNSCH
**Motor de Base de Datos:** PostgreSQL (Alojado en Supabase)
**ORM (Mapeo Objeto-Relacional):** Entity Framework Core (Code-First)

## 1. Diccionario de Datos (Entidades y Atributos)

### 1.1. Módulo de Seguridad y Personal
**Tabla: `Roles`** (Catálogo fijo para definir permisos)
* `Id` (PK, INT): Identificador único (Ej. 1 = Admin, 2 = Bibliotecario).
* `Nombre` (VARCHAR 50, UNIQUE, NOT NULL): Nombre del rol.

**Tabla: `Usuarios`** (Personal que opera el sistema)
* `Id` (PK, UUID): Identificador único seguro autogenerado.
* `Dni` (VARCHAR 8, UNIQUE, NOT NULL): Documento de identidad.
* `Nombres` (VARCHAR 100, NOT NULL): Nombres y apellidos del trabajador.
* `Email` (VARCHAR 100, UNIQUE, NOT NULL): Correo institucional para el login.
* `PasswordHash` (VARCHAR 255, NOT NULL): Contraseña encriptada.
* `PisoArea` (VARCHAR 50, NOT NULL): Zona asignada (Ej. "Piso 1 - Ciencias").
* `RolId` (FK, INT, NOT NULL): Relación con la tabla Roles.
* `Activo` (BOOLEAN, DEFAULT TRUE): Para inhabilitar cuentas sin borrarlas.

### 1.2. Módulo de Inventario
**Tabla: `Categorias`** (Catálogo de áreas de estudio)
* `Id` (PK, INT, IDENTITY): Identificador autoincremental.
* `Nombre` (VARCHAR 100, UNIQUE, NOT NULL): Ej. "Ingeniería de Sistemas", "Derecho".

**Tabla: `Libros`** (Catálogo de ejemplares)
* `Id` (PK, INT, IDENTITY): Identificador autoincremental.
* `Isbn` (VARCHAR 20, UNIQUE, NOT NULL): Código internacional del libro.
* `Titulo` (VARCHAR 200, NOT NULL): Título completo.
* `Autor` (VARCHAR 150, NOT NULL): Nombre del autor principal.
* `CategoriaId` (FK, INT, NOT NULL): Relación con Categorías.
* `StockTotal` (INT, NOT NULL): Cantidad física real en la biblioteca.
* `StockDisponible` (INT, NOT NULL): Cantidad libre para prestar.

### 1.3. Módulo de Padrón Estudiantil
**Tabla: `EscuelasProfesionales`** (Catálogo de carreras UNSCH)
* `Id` (PK, INT, IDENTITY): Identificador autoincremental.
* `Nombre` (VARCHAR 150, UNIQUE, NOT NULL): Ej. "Ingeniería de Sistemas".
* `Facultad` (VARCHAR 150, NOT NULL): Ej. "FIMGC".

**Tabla: `Estudiantes`** (Usuarios finales que se llevan los libros)
* `Id` (PK, INT, IDENTITY): Identificador autoincremental.
* `CodigoUniversitario` (VARCHAR 10, UNIQUE, NOT NULL): Serie UNSCH (Ej. 17180000).
* `Dni` (VARCHAR 8, UNIQUE, NOT NULL): Documento de identidad.
* `NombresCompletos` (VARCHAR 150, NOT NULL): Nombre del estudiante.
* `EscuelaId` (FK, INT, NOT NULL): Relación con Escuelas Profesionales.
* `EsMoroso` (BOOLEAN, DEFAULT FALSE): Bandera automática de penalización.

### 1.4. Módulo Transaccional (Core)
**Tabla: `Prestamos`** (Registro histórico y activo)
* `Id` (PK, UUID): Identificador de la transacción.
* `LibroId` (FK, INT, NOT NULL): El libro que se está prestando.
* `EstudianteId` (FK, INT, NOT NULL): A quién se le presta.
* `UsuarioId` (FK, UUID, NOT NULL): El bibliotecario que registró la operación en ventanilla.
* `FechaHoraPrestamo` (TIMESTAMP, DEFAULT NOW()): Momento exacto de salida.
* `FechaHoraLimite` (TIMESTAMP, NOT NULL): Fecha y hora máxima acordada (Ingresada manualmente por el bibliotecario).
* `FechaHoraDevolucion` (TIMESTAMP, NULL): Momento exacto en que se devuelve (Nulo mientras no se entregue).
* `Estado` (VARCHAR 20, NOT NULL): Estados permitidos: "Activo", "Devuelto", "Vencido".

---

## 2. Cardinalidad y Relaciones (Diagrama ER Lógico)
* **1 a Muchos (1:N):**
  * Un `Rol` tiene muchos `Usuarios`.
  * Una `Categoria` tiene muchos `Libros`.
  * Una `EscuelaProfesional` tiene muchos `Estudiantes`.
  * Un `Libro` puede estar en muchos `Prestamos` (históricamente).
  * Un `Estudiante` puede tener muchos `Prestamos`.
  * Un `Usuario` (Bibliotecario) registra muchos `Prestamos`.

## 3. Restricciones de Integridad y Reglas de Negocio (DB Constraints)
* **CHK_Stock_Positivo:** Se debe configurar un `CHECK CONSTRAINT` en PostgreSQL para garantizar que `StockDisponible >= 0`. La base de datos arrojará error si un préstamo intenta dejar el stock en negativo.
* **CHK_Fechas_Logicas:** Se debe garantizar que `FechaHoraLimite` sea estrictamente mayor a `FechaHoraPrestamo`.
* **Regla de Integridad Referencial:** Los préstamos no pueden ser eliminados físicamente (`ON DELETE RESTRICT`) si están vinculados a un Libro o Estudiante, para no romper el historial de auditoría académica.