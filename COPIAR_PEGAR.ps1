# INSTRUCCIONES PARA COPIAR Y PEGAR EN POWERSHELL

# Paso 1: Navega a la carpeta del proyecto
cd "D:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech"

# Paso 2: Limpia build anterior
dotnet clean

# Paso 3: Compila
dotnet build

# Paso 4: Ejecuta la aplicación
dotnet run --project BiblioTech.API

# Verás algo como:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:7001

# Paso 5: En otra ventana del navegador, abre:
# https://localhost:7001/estudiantes.html
# (Reemplaza 7001 con el puerto que veas en PowerShell si es diferente)

# Paso 6: Presiona F12 en el navegador y abre la Consola

# Paso 7: Verifica que ves estos logs en la consola:
# Iniciando cargarEstudiantes()...
# Response status: 200
# Response ok: true
# Datos recibidos de la API: [{"id":999,"codigoUniversitario":"20240001",...

# Paso 8: En la tabla de la página debe aparecer:
# DNI: 12345678
# CÓDIGO: 20240001
# NOMBRES COMPLETOS: JUAN PÉREZ GARCÍA (DATO DE PRUEBA)
# ESCUELA: Ingeniería de Sistemas (DATO DE PRUEBA)
# ESTADO: HABILITADO

# Si ves esto -> BUG SOLUCIONADO
