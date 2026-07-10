# FASE 1: Análisis y Requerimientos del Sistema

**Proyecto:** BiblioTech - Sistema de Gestión de Biblioteca Central UNSCH

# FUNCIONALES

**R1 Módulo de Inicio de Sesión**

R11 El sistema debe permitir que el administrador y el personal bibliotecario inicien sesión usando sus correos y contraseñas.
R12 El sistema debe permitir al administrador crear cuentas para el personal, asignándoles su rol y el piso/área de atención.
R13 El sistema debe gestionar el acceso a las funcionalidades del sistema dependiendo del rol (Admin o Personal).
R14 El sistema debe permitir cerrar la sesión de forma segura.

R2 Módulo de Inventario
R21 El sistema debe permitir al personal registrar nuevos libros con su Título, Autor, ISBN, Categoría y Cantidad Total.
R22 El sistema debe permitir visualizar y buscar libros disponibles en tiempo real.
R23 El sistema debe permitir actualizar la información de los libros existentes.

R3 Módulo de Padrón de Estudiantes
R31 El sistema debe permitir registrar a los estudiantes indicando su Código Universitario, DNI, Nombres y Escuela Profesional.
R32 El sistema debe permitir validar visualmente si un estudiante se encuentra habilitado o en estado moroso antes de cualquier trámite.

R4 Módulo de Préstamos y Devoluciones
R41 El sistema debe permitir registrar un préstamo de libro a un estudiante habilitado, restando 1 al stock disponible automáticamente.
R42 El sistema debe permitir al personal ingresar manualmente la fecha y hora de devolución acordada con el estudiante al momento del préstamo.
R43 El sistema debe denegar el registro de un préstamo si el libro seleccionado tiene stock 0 o si el estudiante es moroso.
R44 El sistema debe permitir registrar la devolución de un libro, sumando 1 al stock disponible.
R45 El sistema debe cambiar el estado del estudiante a "Moroso" de forma automática si no devuelve el libro pasada la fecha y hora registrada.

R5 Módulo de Dashboard y Reportes
R51 El sistema debe mostrar un panel con la cantidad de préstamos activos, devoluciones pendientes de hoy y alertas de libros sin stock.
R52 El sistema debe permitir al administrador visualizar un reporte de los libros más prestados y la lista de estudiantes morosos.

NO FUNCIONALES

R1 Módulo de Inicio de Sesión
R11 El sistema debe proteger las contraseñas de los usuarios en la base de datos mediante encriptación segura.
R12 El sistema debe restringir el acceso a los módulos en menos de 1 segundo tras la validación de credenciales.

R2 Módulo de Inventario
R21 El sistema debe garantizar la consistencia e integridad matemática del stock de libros en todo momento.

R3 Módulo de Padrón de Estudiantes
R31 El sistema debe permitir la interoperabilidad fluida con la base de datos relacional para cargar la información de los estudiantes sin demoras.

R4 Módulo de Préstamos y Devoluciones
R41 El sistema debe procesar y registrar una transacción de préstamo o devolución en un tiempo menor a 1 segundo.
R42 El sistema debe ser capaz de manejar transacciones simultáneas (ej. piso 1 y piso 2 prestando libros a la vez) sin bloquear la base de datos.
R43 La lógica interna de este módulo debe ser evaluada mediante pruebas unitarias con la tecnología MSTest.
R44 El sistema debe alcanzar una cobertura de código (Code Coverage) mayor al 90% en sus pruebas unitarias verificables en Visual Studio.

R5 Módulo de Dashboard y Reportes
R51 El sistema debe ser fácil de usar, con una estética minimalista, textos en español e interfaces intuitivas para el personal.
R52 El sistema debe cargar y mostrar los datos del dashboard en un tiempo de respuesta promedio que no supere los 3 segundos.

R53 El sistema debe permitir al administrador descargar un reporte rápido de estudiantes morosos en formato CSV mediante un botón intuitivo en la interfaz.

R54 El sistema debe incluir un botón de "Políticas de Préstamo" exclusivo en la barra de navegación del MODO PERSONAL (Bibliotecario) que despliegue una ventana emergente (modal o alerta) detallando de forma clara los términos del servicio, específicamente el límite de tiempo de entrega y la restricción de un máximo de 3 libros prestados por estudiante.
