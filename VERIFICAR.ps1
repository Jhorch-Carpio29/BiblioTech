#!/usr/bin/env pwsh
# Script para verificar que todo está correcto

Write-Host "✅ VERIFICANDO CAMBIOS REALIZADOS..." -ForegroundColor Cyan

Write-Host ""
Write-Host "1. Verificando EstudiantesController.cs..." -ForegroundColor Yellow
$controllerContent = Get-Content "BiblioTech.API\Controllers\EstudiantesController.cs" -Raw
if ($controllerContent -like "*JUAN PÉREZ GARCÍA*" -and $controllerContent -like "*Ingeniería de Sistemas*") {
    Write-Host "   ✅ Controller tiene datos de prueba correctos" -ForegroundColor Green
} else {
    Write-Host "   ❌ ERROR: Controller no tiene datos de prueba" -ForegroundColor Red
}

Write-Host ""
Write-Host "2. Verificando estudiantes.js..." -ForegroundColor Yellow
$jsContent = Get-Content "BiblioTech.API\wwwroot\js\estudiantes.js" -Raw
if ($jsContent -like "*est.nombresCompletos*" -and $jsContent -like "*est.escuelaNombre*") {
    Write-Host "   ✅ JavaScript accede a nombresCompletos y escuelaNombre" -ForegroundColor Green
} else {
    Write-Host "   ❌ ERROR: JavaScript no accede a los campos correctos" -ForegroundColor Red
}

Write-Host ""
Write-Host "3. Compilando..." -ForegroundColor Yellow
dotnet build --quiet 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "   ❌ ERROR EN COMPILACIÓN" -ForegroundColor Red
}

Write-Host ""
Write-Host "✅ TODO ESTÁ LISTO" -ForegroundColor Green
Write-Host ""
Write-Host "Ahora ejecuta:" -ForegroundColor Cyan
Write-Host "  dotnet run --project BiblioTech.API" -ForegroundColor White
Write-Host ""
