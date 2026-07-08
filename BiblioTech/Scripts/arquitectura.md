# FASE 3: Arquitectura de Software y Ecosistema Tecnológico
**Proyecto:** BiblioTech - Sistema de Gestión de Biblioteca Central UNSCH
**Patrón Arquitectónico Principal:** Arquitectura en Capas (Clean Architecture adaptada)

## 1. Ecosistema Tecnológico
* **Backend Framework:** .NET 10 (C#)
* **Motor de Base de Datos:** PostgreSQL (BaaS: Supabase)
* **ORM:** Entity Framework Core (Proveedor: `Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Testing Framework:** MSTest + Moq (Para simulación de datos)
* **Documentación API:** Swagger (OpenAPI 3.0)

---

## 2. Estructura de la Solución (Visual Studio)
La solución `BiblioTech.sln` se dividirá en 5 proyectos (capas) estrictamente desacoplados para garantizar la mantenibilidad y la alta comprobabilidad del código.

### 2.1. Capa de Dominio (`BiblioTech.Domain`)
* **Propósito:** Es el corazón del sistema. No tiene dependencias de ningún otro proyecto ni base de datos.
* **Contenido:** * Entidades POCO (Modelos exactos definidos en `BD.md`: `Usuario`, `Libro`, `Estudiante`, `Prestamo`, etc.).
  * Interfaces de los Repositorios (`IPrestamoRepository`, `ILibroRepository`).
  * Excepciones personalizadas del dominio (Ej. `StockInsuficienteException`, `EstudianteMorosoException`).

### 2.2. Capa de Aplicación (`BiblioTech.Application`)
* **Propósito:** Contiene las reglas de negocio y los casos de uso definidos en el `analisis.md`.
* **Contenido:**
  * **Servicios (Services):** Clases que ejecutan la lógica (Ej. `PrestamoService.cs` que verifica si el estudiante es moroso antes de restar el stock).
  * **DTOs (Data Transfer Objects):** Objetos para enviar y recibir datos limpios sin exponer las entidades reales de la base de datos.

### 2.3. Capa de Infraestructura (`BiblioTech.Infrastructure`)
* **Propósito:** Se encarga exclusivamente de la comunicación con el mundo exterior (PostgreSQL en Supabase).
* **Contenido:**
  * `BiblioTechDbContext.cs` (Configuración de Entity Framework Core).
  * Implementación de los Repositorios (Consultas LINQ reales a la base de datos).
  * Configuraciones de conexión (Connection Strings).

### 2.4. Capa de Presentación / API (`BiblioTech.API`)
* **Propósito:** Exponer el sistema hacia el frontend mediante endpoints RESTful (Rutas web).
* **Contenido:**
  * **Controladores (Controllers):** Rutas POST, GET, PUT, DELETE (Ej. `PrestamosController`).
  * **Swagger:** Interfaz gráfica autogenerada para documentar, visualizar y probar los endpoints manualmente sin necesidad de tener el frontend construido.
  * Middleware de autenticación JWT para proteger las rutas según el Rol (Admin o Personal).

### 2.5. Capa de Pruebas (`BiblioTech.Tests`)
* **Propósito:** Proyecto exclusivo de MSTest para garantizar la calidad del software.
* **Estrategia para >90% Code Coverage:** Se probará intensivamente la capa `BiblioTech.Application`. Utilizando bibliotecas como `Moq`, se simularán las respuestas de la base de datos para probar la lógica matemática y de validación en aislamiento, asegurando ejecuciones rápidas y cobertura total de las reglas de negocio.

---

## 3. Flujo de Datos Transaccional (Ejemplo: Registrar Préstamo)
1. **Frontend** envía un JSON (DTO) vía `POST` al endpoint `/api/prestamos` en la capa **API**.
2. El **Controlador (API)** recibe la petición, valida el token JWT del bibliotecario y pasa los datos al **Servicio (Application)**.
3. El **Servicio** aplica la lógica (Módulo 4 de `analisis.md`): Verifica si el estudiante es moroso y comprueba el `StockDisponible`.
4. Si la lógica falla, lanza un error. Si es exitosa, el Servicio llama al **Repositorio (Infrastructure)**.
5. El **Repositorio** usa EF Core para insertar el registro en Supabase (PostgreSQL) y actualizar el stock del libro.
6. La base de datos confirma, y la **API** devuelve un código `HTTP 201 Created` al frontend.