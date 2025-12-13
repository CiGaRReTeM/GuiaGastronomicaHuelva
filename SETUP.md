# Guía Gastronómica Justa - Instrucciones de instalación

## Requisitos previos

### 1. Instalar .NET 8 SDK

**Windows:**
1. Descarga .NET 8 SDK desde: https://dotnet.microsoft.com/download/dotnet/8.0
2. Ejecuta el instalador y sigue las instrucciones
3. Abre PowerShell y verifica la instalación:
   ```powershell
   dotnet --version
   ```
   Deberías ver `8.0.x` o superior

**macOS/Linux:**
```bash
# macOS (con Homebrew)
brew install dotnet-sdk

# Linux (Ubuntu/Debian)
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

## Ejecutar el proyecto

### 1. Restaurar dependencias

```powershell
cd C:\GuiaGastronomicaHuelva
dotnet restore
```

### 2. Crear la base de datos

```powershell
cd src\GuiaGastronomica.Api
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Si `dotnet ef` no está instalado:
```powershell
dotnet tool install --global dotnet-ef
```

### 3. Ejecutar la API (backend)

```powershell
cd src\GuiaGastronomica.Api
dotnet run
```

La API estará disponible en:
- HTTPS: https://localhost:7001
- HTTP: http://localhost:5001
- Swagger: https://localhost:7001/swagger

### 4. Ejecutar el cliente Blazor (frontend)

Abre una **nueva terminal** PowerShell:

```powershell
cd C:\GuiaGastronomicaHuelva\src\GuiaGastronomica.Client
dotnet run
```

El cliente estará disponible en:
- HTTPS: https://localhost:5002
- HTTP: http://localhost:5003

### 5. Acceder a la aplicación

Abre tu navegador en: **https://localhost:5002**

## Solución de problemas

### Error: "No .NET SDKs were found"
- Instala .NET 8 SDK (ver sección de requisitos)
- Reinicia tu terminal después de la instalación

### Error: "dotnet ef not found"
```powershell
dotnet tool install --global dotnet-ef
```

### Error de CORS en el navegador
- Verifica que la API esté ejecutándose en https://localhost:7001
- Verifica la configuración de CORS en `Program.cs` de la API

### Puerto ya en uso
Cambia el puerto en `Properties/launchSettings.json` de cada proyecto

## Estructura del proyecto

```
GuiaGastronomicaHuelva/
├── src/
│   ├── GuiaGastronomica.Api/         # Backend ASP.NET Core
│   │   ├── Controllers/              # Endpoints REST
│   │   ├── Data/                     # DbContext
│   │   └── Program.cs                # Configuración
│   ├── GuiaGastronomica.Client/      # Frontend Blazor WASM
│   │   ├── Pages/                    # Páginas Razor
│   │   ├── Shared/                   # Componentes compartidos
│   │   └── wwwroot/                  # Archivos estáticos
│   └── GuiaGastronomica.Shared/      # Modelos compartidos
│       ├── Models/                   # Entidades
│       └── DTOs/                     # Data Transfer Objects
├── GUIDE.md                          # Documento de diseño v1.0
└── README.md                         # Instrucciones de instalación
```

## Próximos pasos

1. ✅ Instalar .NET 8 SDK
2. ✅ Restaurar dependencias y compilar
3. ✅ Crear base de datos con EF Core
4. ✅ Ejecutar API y cliente
5. 🔨 Agregar datos de ejemplo (seed data)
6. 🔨 Implementar Semantic Kernel y RAG
7. 🔨 Prototipar chatbot con Ollama
8. 🔨 Implementar ingestión de datos (OSM, scraping)

## Documentación adicional

- Ver `GUIDE.md` para arquitectura completa y roadmap
- Swagger API: https://localhost:7001/swagger (cuando la API esté ejecutándose)

## Soporte

Para problemas o dudas, abre un issue en el repositorio de GitHub.
