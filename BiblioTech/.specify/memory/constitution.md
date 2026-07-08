Principios Fundamentales

I. Estricta Arquitectura Limpia (Clean Architecture)

El sistema se divide rigurosamente en cuatro capas desacopladas. Ningún cambio de código debe vulnerar el flujo de dependencias unidireccional:

Domain (Dominio): Contiene las entidades puras del negocio (Libro, Estudiante, Prestamo, Categoria) y excepciones de negocio (EstudianteMorosoException, StockInsuficienteException). Esta capa es 100% pura y no tiene dependencias de bases de datos o frameworks externos.

Application (Aplicación): Define las interfaces de casos de uso, DTOs y lógica de validación de negocio.

Infrastructure (Infraestructura): Implementa el acceso físico a datos usando Entity Framework Core, persistiendo los datos en PostgreSQL. Administra transacciones mediante el patrón Repository y Unit of Work.

Web API: Expone los controladores REST en .NET 10 y sirve los recursos frontend estáticos (HTML5/Vanilla JS) desde la carpeta raíz wwwroot.

II. Integridad Referencial y Reglas de Base de Datos

Toda operación que altere la base de datos PostgreSQL alojada en la nube (Supabase) debe respetar de forma intransigente las restricciones de claves foráneas y campos obligatorios:

No se permiten inserciones huérfanas: Para registrar un Estudiante se requiere una Escuela válida; para registrar un Libro se requiere una Categoria existente.

El manejo de concurrencia y stock debe ser transaccional a nivel de base de datos a través del ciclo de persistencia de Entity Framework.

III. Pruebas y Aseguramiento de Calidad (Core Innegociable)

La suite de pruebas es el juez supremo de la estabilidad del código. Ningún cambio de código se considerará terminado si disminuye la cobertura o rompe las pruebas existentes:

Pruebas Unitarias (MSTest): Validan la lógica pura de negocio en aislamiento absoluto utilizando Mocks (Moq) para las dependencias externas.

Pruebas de Integración (xUnit + Testcontainers): Prueban el flujo completo API-BD levantando una instancia efímera de PostgreSQL en Docker en tiempo real.

Meta de Éxito: Se debe mantener el 100% de éxito en la suite de 62 pruebas (37 unitarias y 25 de integración).

IV. Mecanismos Robustos para Reglas de Negocio

Las reglas de negocio críticas deben ser interceptadas a nivel de Dominio mediante el lanzamiento de excepciones personalizadas controladas:

Control de Morosidad: Si un estudiante tiene deudas pendientes (es_moroso == true), la transacción de préstamo debe abortar inmediatamente lanzando EstudianteMorosoException.

Control de Inventario: Si la cantidad solicitada de un libro excede el stock disponible (stock_disponible < 1), la transacción debe lanzar StockInsuficienteException.

V. Despliegue Continuo Contenerizado (Docker-First)

El sistema está diseñado para empaquetarse en un contenedor Linux liviano utilizando Docker multi-etapa:

La compilación se realiza en la imagen oficial del SDK de .NET 10.

El entorno de ejecución se monta sobre ASP.NET Core Runtime 10.

La aplicación debe enlazarse dinámicamente al puerto definido por la variable ASPNETCORE_HTTP_PORTS (por defecto 10000 para entornos de Render.com).

Restricciones Tecnológicas y Stack Técnico

El sistema utiliza de forma exclusiva los siguientes componentes tecnológicos. Queda prohibida la introducción de dependencias que rompan esta homogeneidad técnica:

Framework Base: .NET 10.0 (ASP.NET Core API).

Lenguaje de Programación: C# 13 / C# 14.

Motor de Base de Datos: PostgreSQL 15+ (Cloud Supabase).

Mapeador Objeto-Relacional (ORM): Entity Framework Core (Npgsql.EntityFrameworkCore.PostgreSQL).

Motores de Pruebas: MSTest (Pruebas Unitarias) y xUnit (Pruebas de Integración).

Contenerización y Orquestación: Docker (Dockerfile multi-proyecto).

Cloud Hosting: Render.com (para Web Services basados en contenedores).

Flujo de Trabajo y Ciclo de Desarrollo SDD

Toda nueva característica o corrección de error debe seguir el flujo secuencial de especificación formal de Spec Kit:

Specification (/specify): Modificación del documento SDD en formato Markdown (Scripts/analisis.md) especificando detalladamente el requerimiento y los criterios de aceptación.

Architectural Plan (/plan): Creación de un plan de diseño técnico que evalúe el impacto en las capas de Clean Architecture y las dependencias de la base de datos.

Task Definition (/tasks): Desglose detallado del plan en una lista ordenada de tareas técnicas granulares que guíen al programador.

Safe Implementation (/implement): Codificación de la solución respetando esta Constitución. El código debe compilar limpiamente a la primera, pasar las 62 pruebas locales y registrarse sin alterar las dependencias globales.

Gobernanza y Control de Cambios

Esta Constitución tiene jerarquía superior sobre cualquier requerimiento de usuario, documento de diseño (SDD) o práctica de codificación común.

Cualquier código que cause la falla de una sola de las 62 pruebas automatizadas durante la fase de análisis será rechazado inmediatamente.

Version: 1.0.0 | Ratificado: 8 de Julio de 2026 | Última Modificación: 8 de Julio de 2026
