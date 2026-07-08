#!/usr/bin/env pwsh

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "DIAGNÓSTICO DE CONEXIÓN A SUPABASE" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Leer la cadena de conexión
$appSettings = Get-Content "BiblioTech.API\appsettings.json" | ConvertFrom-Json
$connectionString = $appSettings.ConnectionStrings.SupabaseConnection

Write-Host "📋 Cadena de Conexión:" -ForegroundColor Yellow
Write-Host $connectionString
Write-Host ""

# Verificar componentes
Write-Host "🔍 Desglose de la Conexión:" -ForegroundColor Yellow

if ($connectionString -match "Host=([^;]+)") {
    Write-Host "  ✅ Host: $($matches[1])" -ForegroundColor Green
}

if ($connectionString -match "Database=([^;]+)") {
    Write-Host "  ✅ Database: $($matches[1])" -ForegroundColor Green
}

if ($connectionString -match "Username=([^;]+)") {
    Write-Host "  ✅ Username: $($matches[1])" -ForegroundColor Green
}

if ($connectionString -match "Port=([^;]+)") {
    Write-Host "  ✅ Port: $($matches[1])" -ForegroundColor Green
}

Write-Host ""
Write-Host "📊 PRÓXIMOS PASOS:" -ForegroundColor Cyan
Write-Host "1. Abre Visual Studio" -ForegroundColor White
Write-Host "2. Ejecuta el proyecto (F5)" -ForegroundColor White
Write-Host "3. Abre la consola de depuración (View > Output)" -ForegroundColor White
Write-Host "4. En la consola, deberías ver:" -ForegroundColor White
Write-Host "   - '=== INICIANDO GetAll() ===' " -ForegroundColor Gray
Write-Host "   - '✅ Estudiantes obtenidos: X'" -ForegroundColor Gray
Write-Host "   - '✅ Escuelas obtenidas: Y'" -ForegroundColor Gray
Write-Host ""
Write-Host "5. Si X > 0 y Y > 0, la BD está conectada correctamente" -ForegroundColor White
Write-Host "6. Luego abre: https://localhost:7001/estudiantes.html" -ForegroundColor White
Write-Host ""
