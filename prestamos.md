# Especificación: Módulo de Préstamos
## Objetivo
Registrar la salida de un libro del inventario y asociarlo a un estudiante, descontando el stock.

## Reglas de Negocio
- SI el estudiante es moroso, lanzar excepción `EstudianteMorosoException`.
- SI el stock del libro es 0, lanzar excepción `StockInsuficienteException`.
- AL registrar el préstamo, restar 1 del `StockDisponible` del libro.
- AL registrar la devolución, sumar 1 al `StockDisponible` del libro.

## Contrato API
- POST /api/prestamos
  - Request: { EstudianteId, LibroId, FechaLimite }
  - Response: 201 Created