# Script para detener la Guía Gastronómica de Huelva

Write-Host "🛑 Deteniendo aplicación..." -ForegroundColor Yellow
Write-Host ""

# Buscar y detener procesos de la aplicación
$processes = Get-Process | Where-Object {
    $_.ProcessName -like "*GuiaGastronomica*" -or 
    ($_.MainWindowTitle -like "*Guía Gastronómica*")
}

if ($processes) {
    Write-Host "Encontrados $($processes.Count) proceso(s) en ejecución:" -ForegroundColor Cyan
    foreach ($proc in $processes) {
        Write-Host "  • $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Gray
    }
    Write-Host ""
    
    $processes | Stop-Process -Force
    Write-Host "✓ Todos los procesos detenidos" -ForegroundColor Green
} else {
    Write-Host "ℹ No se encontraron procesos en ejecución" -ForegroundColor Gray
}

Write-Host ""
Write-Host "✅ Aplicación detenida" -ForegroundColor Green
Write-Host ""
