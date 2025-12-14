# 🚀 Scripts de Ejecución

## Scripts Disponibles

### `run.ps1` - Iniciar Aplicación
Inicia la API Backend y el Cliente Blazor en ventanas separadas, y abre el navegador automáticamente.

**Uso:**
```powershell
.\run.ps1
```

**Qué hace:**
1. Verifica que .NET SDK esté instalado
2. Verifica que los proyectos existan
3. Inicia la API en http://localhost:5001
4. Inicia el Cliente en http://localhost:5002
5. Abre el navegador automáticamente

---

### `stop.ps1` - Detener Aplicación
Detiene todos los procesos de la aplicación en ejecución.

**Uso:**
```powershell
.\stop.ps1
```

**Qué hace:**
1. Busca todos los procesos relacionados con la aplicación
2. Los detiene de forma forzada
3. Confirma que todo se detuvo correctamente

---

## Ejecución Manual (Sin Scripts)

Si prefieres ejecutar manualmente sin los scripts:

### Terminal 1 - API:
```powershell
cd C:\GuiaGastronomicaHuelva\src\GuiaGastronomica.Api
dotnet run
```

### Terminal 2 - Cliente:
```powershell
cd C:\GuiaGastronomicaHuelva\src\GuiaGastronomica.Client
dotnet run
```

### Abrir en navegador:
http://localhost:5002

---

## Solución de Problemas

### Error: "No se puede ejecutar scripts en este sistema"
Si recibes un error de política de ejecución, ejecuta:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Los puertos ya están en uso
Si los puertos 5001 o 5002 están ocupados, ejecuta `stop.ps1` primero, o reinicia tu computadora.

### La API o el Cliente no inician
Verifica que:
- .NET 8 SDK esté instalado: `dotnet --version`
- Los proyectos compilen: `dotnet build`
- La base de datos exista: `src/GuiaGastronomica.Api/guiagastronomica.db`

---

## URLs Importantes

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Cliente** | http://localhost:5002 | Interfaz web principal |
| **API** | http://localhost:5001 | Backend REST API |
| **Swagger** | http://localhost:5001/swagger | Documentación interactiva API |

---

## Próximos Pasos

Una vez la aplicación esté corriendo:
1. Explora la interfaz en http://localhost:5002
2. Revisa la documentación API en http://localhost:5001/swagger
3. Agrega datos de prueba (venues, reviews)
4. Implementa el chatbot con IA
