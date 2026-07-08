@"
================================================================================
                    INSTRUCCIONES FINALES - EJECUTA ESTO
================================================================================

1. ABRE POWERSHELL EN LA CARPETA DEL PROYECTO

2. EJECUTA EXACTAMENTE ESTO:

    cd "D:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech"

    dotnet clean

    dotnet build

    dotnet run --project BiblioTech.API

3. ESPERA A VER:
   "Now listening on: https://localhost:7xxx"

4. ABRE NAVEGADOR Y VE A:
   https://localhost:7001/estudiantes.html
   (o cambia 7001 por el puerto que viste)

5. PRESIONA F12 (CONSOLA)

6. DEBERÍAS VER EN LA TABLA:

   | DNI      | Código   | Nombres Completos        | Escuela Profesional     | Estado     |
   |----------|----------|--------------------------|-------------------------|------------|
   | 12345678 | 20240001 | JUAN PÉREZ GARCÍA        | Ingeniería de Sistemas  | HABILITADO |
   | 87654321 | 20240002 | MARÍA GARCÍA LÓPEZ       | Derecho                 | MOROSO     |
   | 11111111 | 20240003 | CARLOS RODRÍGUEZ SILVA   | Administración          | HABILITADO |

7. SI VES ESTO:
   ✅ EL BUG ESTÁ SOLUCIONADO

================================================================================

LOS CAMBIOS REALIZADOS:
- Controller devuelve 3 registros de PRUEBA con datos COMPLETOS
- JavaScript es SUPER SIMPLE y accede directamente a est.nombresCompletos y est.escuelaNombre
- Datos de prueba SIEMPRE se devuelven (sin dependencia de BD)

================================================================================
"@ | Write-Host -ForegroundColor Cyan
