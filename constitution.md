# Constitución de BiblioTech
- Todo código debe seguir la Clean Architecture (Domain, Application, Infrastructure, API).
- Todo servicio en `Application` debe tener su clase de pruebas unitarias en `Tests`.
- La persistencia se hace vía EF Core con Fluent API.
- Los endpoints deben seguir estándares RESTful (201 para creación, 200 para consulta).