# Script para ejecutar la Guia Gastronomica de Huelva
# Ejecuta la API y el Cliente Blazor en ventanas separadas

Write-Host "Iniciando Guia Gastronomica de Huelva..." -ForegroundColor Green
Write-Host ""

# Verificar que .NET SDK esta instalado
try {
    $dotnetVersion = dotnet --version
    Write-Host "[OK] .NET SDK detectado: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] .NET SDK no encontrado. Instala .NET 8 SDK primero." -ForegroundColor Red
    exit 1
}

# Verificar que los proyectos existen
$apiPath = "C:\GuiaGastronomicaHuelva\src\GuiaGastronomica.Api"
$clientPath = "C:\GuiaGastronomicaHuelva\src\GuiaGastronomica.Client"

if (-not (Test-Path "$apiPath\GuiaGastronomica.Api.csproj")) {
    Write-Host "[ERROR] No se encuentra el proyecto API" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path "$clientPath\GuiaGastronomica.Client.csproj")) {
    Write-Host "[ERROR] No se encuentra el proyecto Cliente" -ForegroundColor Red
    exit 1
}

Write-Host "[OK] Proyectos encontrados" -ForegroundColor Green
Write-Host ""

# Iniciar API en ventana separada
Write-Host "Iniciando API Backend (http://localhost:5000)..." -ForegroundColor Cyan
$apiCommand = "cd `"$apiPath`"; dotnet run"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $apiCommand

# Esperar a que la API inicie
Write-Host "   Esperando a que la API inicie..." -ForegroundColor Gray
Start-Sleep -Seconds 5

# Iniciar Cliente en ventana separada
Write-Host "Iniciando Cliente Blazor (http://localhost:5002)..." -ForegroundColor Cyan
$clientCommand = "cd `"$clientPath`"; dotnet run"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $clientCommand

# Esperar a que el cliente inicie
Write-Host "   Esperando a que el cliente inicie..." -ForegroundColor Gray
Start-Sleep -Seconds 8

# Abrir navegador
Write-Host ""
Write-Host "Abriendo navegador en http://localhost:5002..." -ForegroundColor Green
Start-Process "http://localhost:5002"

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "Aplicacion iniciada correctamente" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "URLs disponibles:" -ForegroundColor Yellow
Write-Host "  - Cliente:       http://localhost:5002" -ForegroundColor White
Write-Host "  - API:           http://localhost:5000" -ForegroundColor White
Write-Host "  - Swagger:       http://localhost:5000/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Para detener la aplicacion:" -ForegroundColor Yellow
Write-Host "  1. Cierra las ventanas de PowerShell de API y Cliente" -ForegroundColor White
Write-Host "  2. O ejecuta el script stop.ps1" -ForegroundColor White
Write-Host ""
