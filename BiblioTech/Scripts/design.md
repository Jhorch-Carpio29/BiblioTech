# FASE 4: Diseño de Interfaz (UI/UX) y Contratos de API
**Proyecto:** BiblioTech - Sistema de Gestión de Biblioteca Central UNSCH
**Enfoque de Diseño:** Neubrutalismo / Modern Tech (Colores vibrantes, alto contraste, sombras duras y máxima personalidad).

## 1. Guía de Estilos Visuales (UI Kit)
* **Tipografía:** `Syne` (para Títulos gigantes y modernos) y `Montserrat` (para lectura clara en tablas y datos).
* **Paleta de Colores (Estilo "Chévere/Vibrante"):**
  * **Color Base/Texto:** `#1C1B1B` (Negro puro para bordes gruesos y textos con mucha fuerza).
  * **Color Primario (Acción):** `#C3F400` (Verde Lima Neón) - Para botones de confirmación y préstamos.
  * **Color Secundario:** `#FF00FF` (Magenta Neón) y `#00F8F8` (Cian Eléctrico) - Para resaltar tarjetas del dashboard, menús activos y botones secundarios.
  * **Fondo (Background):** `#FCF9F8` (Blanco tiza) - Para dar un contraste brutal con los colores neón sin cansar la vista.
  * **Alertas (Feedback con estilo):** * Éxito/Activo: `#00EA80` (Verde Esmeralda brillante)
    * Peligro / Moroso: `#FF3366` (Rojo Fresa vibrante)
    * Advertencia / Stock bajo: `#FF9F00` (Naranja Fuego)

## 2. Estructura General del Layout (Plantilla Base)
Todas las pantallas (excepto el Login) compartirán una estructura tipo **Dashboard**:
* **Sidebar (Izquierda - Fija):** Fondo blanco con borde derecho negro grueso. Logo de BiblioTech en grande, Nombre del usuario, y menú de navegación. El botón activo de la página actual se pintará de un color vibrante (Cian o Magenta) con sombra negra rígida.
* **Header (Arriba):** Buscador rápido (por DNI o ISBN) con diseño de caja fuerte (borde negro) y notificaciones de sistema.
* **Main Content (Centro):** Tarjetas de contenido con fondos neón, tablas de datos limpias pero con cabeceras negras y letras blancas.

## 3. Especificación de Vistas (Pantallas Frontend)

### 3.1. `index.html` (Inicio de Sesión)
* **Diseño:** Pantalla llamativa. Mitad izquierda con un patrón visual tecnológico; mitad derecha con el formulario en una caja flotante con sombra negra dura.
* **Elementos:** Input de Email, Input de Password, Botón gigante "Ingresar al Sistema" en Verde Lima.

### 3.2. `dashboard.html` (Panel de Control)
* **Diseño:** 3 Tarjetas superiores de resumen gigantes y coloridas (Cian para préstamos, Magenta para devoluciones, Naranja para alertas).
* **Elementos:** Dos tablas en la parte inferior: "Últimos Movimientos" y "Estudiantes Morosos" (con badges Rojo Fresa para los morosos).

### 3.3. `inventario.html` (Gestión de Libros)
* **Diseño:** Cabecera con botón "+ Nuevo Libro" en Cian Eléctrico. Debajo, una tabla de datos (DataGrid).
* **Elementos de Tabla:** Título, Autor, Cód. Categoría, Stock Total, **Stock Disponible (Badge vibrante)**, Acciones (Editar/Borrar).

### 3.4. `estudiantes.html` (Padrón)
* **Diseño:** Tabla de datos con buscador por DNI o Código universitario súper visible.
* **Elementos de Tabla:** DNI, Nombres, Escuela, **Estado (Badge Verde Esmeralda para Activo, Rojo Fresa para Moroso)**.

### 3.5. `prestamos.html` (Centro de Transacciones)
* **Diseño:** Vista centralizada e interactiva.
* **Proceso de Préstamo:** Input para DNI del estudiante (valida al instante si es moroso), Input para ISBN del libro. Selector de Fecha y Hora límite. Botón gigante "Confirmar Préstamo" (Verde Lima).
* **Proceso de Devolución:** Lista de préstamos activos en tarjetas. Botón "Registrar Devolución" al lado de cada registro.

---

## 4. Contratos de Comunicación (Frontend <-> Backend API)
El Frontend consumirá las APIs del backend (.NET) utilizando peticiones `fetch()` con formato JSON. Los principales endpoints documentados en Swagger serán:

* **Autenticación:**
  * `POST /api/auth/login` -> Retorna Token JWT.
* **Inventario:**
  * `GET /api/libros` -> Lista de libros.
  * `POST /api/libros` -> Registra un libro.
* **Estudiantes:**
  * `GET /api/estudiantes/{dni}` -> Trae datos y estado del estudiante.
* **Préstamos:**
  * `POST /api/prestamos` -> Registra préstamo (Resta stock).
  * `PUT /api/prestamos/devolucion/{id}` -> Registra entrega (Suma stock).