# 📚 Guía de Desarrollo: Blazor WebAssembly + MudBlazor

## Índice
1. [¿Qué es Blazor WebAssembly?](#qué-es-blazor-webassembly)
2. [Estructura del Proyecto Cliente](#estructura-del-proyecto-cliente)
3. [Componentes Razor](#componentes-razor)
4. [MudBlazor - Biblioteca de Componentes UI](#mudblazor)
5. [Conceptos Clave de Blazor](#conceptos-clave)
6. [Ejemplo Práctico: Venues.razor](#ejemplo-práctico)
7. [Recursos Adicionales](#recursos-adicionales)

---

## ¿Qué es Blazor WebAssembly?

**Blazor WebAssembly** es un framework de Microsoft para crear aplicaciones web SPA (Single Page Application), similar a React, Vue o Angular, pero con las siguientes características únicas:

- ✅ Usas **C#** en lugar de JavaScript
- ✅ Se ejecuta en el **navegador** usando WebAssembly
- ✅ Comparte código con el backend (.NET)
- ✅ Acceso completo al ecosistema .NET (NuGet, LINQ, async/await, etc.)
- ✅ Tipado fuerte y compilación en tiempo de compilación

**Ventajas:**
- Un solo lenguaje (C#) para frontend y backend
- Reutilización de modelos y DTOs
- Performance nativa del navegador (WebAssembly)
- Intellisense completo en Visual Studio/VS Code

---

## Estructura del Proyecto Cliente

### 📁 Jerarquía de Archivos

```
GuiaGastronomica.Client/
├── wwwroot/
│   └── index.html              # Punto de entrada HTML (único archivo HTML)
├── Pages/
│   ├── Home.razor              # Página de inicio (@page "/")
│   └── Venues.razor            # Página de locales (@page "/venues")
├── Shared/
│   ├── MainLayout.razor        # Layout base de la aplicación
│   └── NavMenu.razor           # Menú de navegación
├── _Imports.razor              # Importaciones globales
├── App.razor                   # Router principal
├── Program.cs                  # Configuración inicial
└── GuiaGastronomica.Client.csproj
```

---

## Archivos Clave del Proyecto

### 1. **wwwroot/index.html** - Punto de Entrada

```html
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <title>Guía Gastronómica Justa - Huelva</title>
    
    <!-- CSS de MudBlazor -->
    <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
</head>
<body>
    <!-- Aquí se monta toda la aplicación Blazor -->
    <div id="app">
        <div style="text-align: center; margin-top: 50px;">
            <h1>Cargando...</h1>
        </div>
    </div>

    <!-- Motor de Blazor WebAssembly -->
    <script src="_framework/blazor.webassembly.js"></script>
    
    <!-- JavaScript de MudBlazor -->
    <script src="_content/MudBlazor/MudBlazor.min.js"></script>
</body>
</html>
```

**Puntos clave:**
- Es el **único archivo HTML** de toda la aplicación
- Blazor inyecta todo el contenido dinámicamente en `<div id="app">`
- Similar a `index.html` en React o Vue

---

### 2. **Program.cs** - Configuración Inicial

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GuiaGastronomica.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Monta el componente App en <div id="app">
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient para llamar a la API
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("http://localhost:5001") 
});

// Registrar servicios de MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
```

**Equivalente a:**
- `main.js` en Vue
- `index.tsx` o `App.tsx` en React
- `main.ts` en Angular

**Funciones principales:**
- Registra componentes raíz
- Configura servicios (Dependency Injection)
- Inicializa la aplicación

---

### 3. **App.razor** - Router Principal

```razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>No encontrado</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Página no encontrada.</p>
        </LayoutView>
    </NotFound>
</Router>
```

**Función:**
- Define el sistema de **ruteo** de la aplicación
- Renderiza páginas según la URL
- Usa `MainLayout` como plantilla base
- Maneja páginas no encontradas (404)

---

## Componentes Razor

Los archivos `.razor` son componentes reutilizables que combinan **HTML + C#**.

### Estructura de un Componente Razor

```razor
@page "/ruta"                    <!-- Directiva de ruta (opcional) -->
@using MudBlazor                 <!-- Importaciones -->
@inject HttpClient Http          <!-- Inyección de dependencias -->

<!-- PARTE HTML/UI -->
<MudContainer>
    <MudText Typo="Typo.h3">@titulo</MudText>
    <MudButton OnClick="HandleClick">Clic</MudButton>
    
    @if (isLoading) {
        <MudProgressCircular />
    }
    
    @foreach (var item in items) {
        <MudText>@item.Name</MudText>
    }
</MudContainer>

<!-- PARTE C# -->
@code {
    // Propiedades
    private string titulo = "Mi Título";
    private bool isLoading = false;
    private List<Item> items = new();
    
    // Ciclo de vida
    protected override async Task OnInitializedAsync() {
        // Se ejecuta cuando el componente se monta
        await LoadData();
    }
    
    // Métodos
    private async Task LoadData() {
        isLoading = true;
        items = await Http.GetFromJsonAsync<List<Item>>("api/items");
        isLoading = false;
    }
    
    private void HandleClick() {
        Console.WriteLine("Botón clickeado");
    }
}
```

---

### MainLayout.razor - Layout Base

```razor
@inherits LayoutComponentBase

<!-- Proveedores de MudBlazor (globales) -->
<MudThemeProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <!-- Barra superior -->
    <MudAppBar Elevation="1">
        <MudIconButton Icon="@Icons.Material.Filled.Menu" 
                       Color="Color.Inherit" 
                       Edge="Edge.Start" 
                       OnClick="@DrawerToggle" />
        <MudText Typo="Typo.h5" Class="ml-3">Guía Gastronómica Justa</MudText>
        <MudSpacer />
        <MudText Typo="Typo.body2">Huelva</MudText>
    </MudAppBar>
    
    <!-- Menú lateral deslizable -->
    <MudDrawer @bind-Open="_drawerOpen" 
               ClipMode="DrawerClipMode.Always" 
               Elevation="2">
        <NavMenu />
    </MudDrawer>
    
    <!-- Contenido principal (aquí se inyecta cada página) -->
    <MudMainContent Class="mt-16 pa-4">
        @Body
    </MudMainContent>
</MudLayout>

@code {
    private bool _drawerOpen = true;
    
    private void DrawerToggle() {
        _drawerOpen = !_drawerOpen;
    }
}
```

**Componentes MudBlazor usados:**
- `MudAppBar`: Barra de navegación superior fija
- `MudDrawer`: Menú lateral con animación
- `MudMainContent`: Contenedor del contenido de cada página
- `@Body`: Placeholder donde se inyecta el contenido de la página actual

---

## MudBlazor - Biblioteca de Componentes UI

**MudBlazor** es una biblioteca de componentes UI estilo Material Design para Blazor, similar a:
- **Vue**: Vuetify
- **React**: Material-UI (MUI)
- **Angular**: Angular Material

### Componentes Más Usados

| Componente | Equivalente HTML | Descripción |
|------------|------------------|-------------|
| `<MudButton>` | `<button>` | Botones con estilos Material |
| `<MudTextField>` | `<input>` | Campos de texto |
| `<MudSelect>` | `<select>` | Desplegables |
| `<MudCard>` | `<div class="card">` | Tarjetas |
| `<MudDataGrid>` | `<table>` | Tablas con filtros, sorting, paginación |
| `<MudDialog>` | Modal | Ventanas emergentes |
| `<MudSnackbar>` | Toast | Notificaciones |
| `<MudAppBar>` | `<header>` | Barra de navegación |
| `<MudDrawer>` | `<aside>` | Menú lateral |

### Ejemplos de Componentes MudBlazor

#### Botones
```razor
<MudButton Variant="Variant.Filled" Color="Color.Primary">
    Primario
</MudButton>
<MudButton Variant="Variant.Outlined" Color="Color.Secondary">
    Secundario
</MudButton>
<MudIconButton Icon="@Icons.Material.Filled.Add" />
```

#### Campos de Texto
```razor
<MudTextField @bind-Value="nombre" 
              Label="Nombre" 
              Variant="Variant.Outlined" 
              Required="true" />
              
<MudTextField @bind-Value="descripcion" 
              Label="Descripción" 
              Lines="5" />
```

#### Tarjetas
```razor
<MudCard Elevation="2">
    <MudCardMedia Image="/images/food.jpg" Height="200" />
    <MudCardContent>
        <MudText Typo="Typo.h6">Título</MudText>
        <MudText Typo="Typo.body2">Descripción</MudText>
    </MudCardContent>
    <MudCardActions>
        <MudButton>Ver más</MudButton>
    </MudCardActions>
</MudCard>
```

#### Grid System (Responsive)
```razor
<MudGrid>
    <MudItem xs="12" sm="6" md="4">
        <!-- Contenido -->
    </MudItem>
    <MudItem xs="12" sm="6" md="4">
        <!-- Contenido -->
    </MudItem>
</MudGrid>
```
- `xs`: Extra small (móviles)
- `sm`: Small (tablets)
- `md`: Medium (desktops)
- `lg`: Large
- `xl`: Extra large

---

## Conceptos Clave de Blazor

### 1. Data Binding (Vinculación de Datos)

#### One-Way Binding (Solo lectura)
```razor
<MudText>Hola, @nombre</MudText>

@code {
    private string nombre = "Juan";
}
```

#### Two-Way Binding (Lectura/Escritura)
```razor
<MudTextField @bind-Value="nombre" Label="Nombre" />
<MudText>Escribiste: @nombre</MudText>

@code {
    private string nombre = "";
}
```

---

### 2. Eventos

```razor
<!-- Evento de clic -->
<MudButton OnClick="HandleClick">Clic aquí</MudButton>

<!-- Evento con parámetros -->
<MudButton OnClick="@(() => DeleteItem(item.Id))">Eliminar</MudButton>

<!-- Evento de cambio -->
<MudTextField @bind-Value="searchText" 
              @bind-Value:after="OnSearchChanged" />

@code {
    private void HandleClick() {
        Console.WriteLine("Botón clickeado");
    }
    
    private void DeleteItem(int id) {
        // Lógica de eliminación
    }
    
    private void OnSearchChanged() {
        // Se ejecuta después de cambiar searchText
    }
}
```

**Eventos comunes:**
- `OnClick`: Click del ratón
- `OnChange`: Cambio de valor
- `@bind-Value:after`: Después de actualizar valor
- `@onkeyup`: Tecla soltada
- `@onmouseover`: Mouse sobre elemento

---

### 3. Renderizado Condicional

```razor
@if (isLoading) {
    <MudProgressCircular Color="Color.Primary" Indeterminate="true" />
    <MudText>Cargando...</MudText>
}
else if (items == null || !items.Any()) {
    <MudAlert Severity="Severity.Info">No hay datos</MudAlert>
}
else {
    <MudText>Mostrando @items.Count elementos</MudText>
}
```

---

### 4. Bucles (Loops)

```razor
<!-- Foreach simple -->
@foreach (var item in items) {
    <MudText>@item.Name</MudText>
}

<!-- Con índice -->
@foreach (var (item, index) in items.Select((value, i) => (value, i))) {
    <MudText>@index: @item.Name</MudText>
}

<!-- Dentro de componentes -->
<MudGrid>
    @foreach (var venue in venues) {
        <MudItem xs="12" sm="6" md="4">
            <MudCard>
                <MudCardContent>
                    <MudText Typo="Typo.h6">@venue.Name</MudText>
                    <MudText>@venue.Address</MudText>
                </MudCardContent>
            </MudCard>
        </MudItem>
    }
</MudGrid>
```

---

### 5. Inyección de Dependencias

```razor
@inject HttpClient Http
@inject NavigationManager Navigation
@inject ISnackbar Snackbar

@code {
    private async Task LoadData() {
        // Usar HttpClient inyectado
        var data = await Http.GetFromJsonAsync<List<Item>>("api/items");
    }
    
    private void GoToHome() {
        // Usar NavigationManager para navegar
        Navigation.NavigateTo("/");
    }
    
    private void ShowMessage() {
        // Mostrar notificación
        Snackbar.Add("¡Operación exitosa!", Severity.Success);
    }
}
```

**Servicios comunes:**
- `HttpClient`: Para llamadas HTTP
- `NavigationManager`: Para navegación programática
- `IJSRuntime`: Para interoperabilidad con JavaScript
- `ISnackbar`: Para notificaciones (MudBlazor)

---

### 6. Llamadas HTTP a la API

```razor
@inject HttpClient Http

@code {
    private List<VenueDto> venues = new();
    private bool isLoading = false;
    private string errorMessage = "";
    
    // GET simple
    private async Task LoadVenues() {
        isLoading = true;
        try {
            venues = await Http.GetFromJsonAsync<List<VenueDto>>("api/venues") 
                     ?? new List<VenueDto>();
        }
        catch (Exception ex) {
            errorMessage = $"Error: {ex.Message}";
        }
        finally {
            isLoading = false;
        }
    }
    
    // GET con parámetros
    private async Task SearchVenues(string zone, string category) {
        var query = $"api/venues?zone={zone}&category={category}";
        venues = await Http.GetFromJsonAsync<List<VenueDto>>(query);
    }
    
    // POST
    private async Task CreateVenue(VenueDto newVenue) {
        var response = await Http.PostAsJsonAsync("api/venues", newVenue);
        if (response.IsSuccessStatusCode) {
            Snackbar.Add("Venue creado", Severity.Success);
        }
    }
    
    // PUT
    private async Task UpdateVenue(int id, VenueDto venue) {
        await Http.PutAsJsonAsync($"api/venues/{id}", venue);
    }
    
    // DELETE
    private async Task DeleteVenue(int id) {
        await Http.DeleteAsync($"api/venues/{id}");
    }
}
```

---

### 7. Ciclo de Vida de Componentes

```razor
@code {
    // 1. Constructor (raramente usado)
    public MyComponent() {
        Console.WriteLine("Constructor ejecutado");
    }
    
    // 2. SetParametersAsync - Cuando se establecen parámetros
    public override async Task SetParametersAsync(ParameterView parameters) {
        await base.SetParametersAsync(parameters);
    }
    
    // 3. OnInitialized - Primera vez que se monta el componente
    protected override void OnInitialized() {
        Console.WriteLine("Componente inicializado (sync)");
    }
    
    // 4. OnInitializedAsync - Primera vez (versión async)
    protected override async Task OnInitializedAsync() {
        // IDEAL PARA: Cargar datos iniciales
        await LoadData();
    }
    
    // 5. OnParametersSet - Cada vez que cambian los parámetros
    protected override void OnParametersSet() {
        Console.WriteLine("Parámetros actualizados");
    }
    
    // 6. OnAfterRender - Después de renderizar
    protected override void OnAfterRender(bool firstRender) {
        if (firstRender) {
            // Solo la primera vez
            Console.WriteLine("Primer renderizado completado");
        }
    }
}
```

**Más usado:** `OnInitializedAsync()` para cargar datos al montar el componente.

---

## Ejemplo Práctico: Venues.razor

Este es un ejemplo real del proyecto:

```razor
@page "/venues"
@using GuiaGastronomica.Shared.DTOs
@inject HttpClient Http

<PageTitle>Locales - Guía Gastronómica Justa</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large" Class="mt-4">
    <MudText Typo="Typo.h3" GutterBottom="true">
        Locales Gastronómicos en Huelva
    </MudText>

    <!-- Filtros de búsqueda -->
    <MudGrid Class="mb-4">
        <MudItem xs="12" sm="6" md="4">
            <MudTextField @bind-Value="@searchZone" 
                          Label="Zona" 
                          Variant="Variant.Outlined" />
        </MudItem>
        <MudItem xs="12" sm="6" md="4">
            <MudTextField @bind-Value="@searchCategory" 
                          Label="Categoría" 
                          Variant="Variant.Outlined" />
        </MudItem>
        <MudItem xs="12" sm="6" md="4">
            <MudButton Variant="Variant.Filled" 
                       Color="Color.Primary" 
                       OnClick="LoadVenues" 
                       FullWidth="true">
                Buscar
            </MudButton>
        </MudItem>
    </MudGrid>

    <!-- Estado de carga -->
    @if (loading) {
        <MudProgressCircular Color="Color.Primary" Indeterminate="true" />
        <MudText>Cargando locales...</MudText>
    }
    else if (venues == null || !venues.Any()) {
        <MudAlert Severity="Severity.Info">
            No se encontraron locales. Pronto agregaremos contenido.
        </MudAlert>
    }
    else {
        <!-- Grid de tarjetas -->
        <MudGrid>
            @foreach (var venue in venues) {
                <MudItem xs="12" sm="6" md="4">
                    <MudCard Elevation="2">
                        <MudCardContent>
                            <MudText Typo="Typo.h6">@venue.Name</MudText>
                            <MudText Typo="Typo.body2">@venue.Address</MudText>
                            <MudText Typo="Typo.body2" Color="Color.Primary">
                                Puntuación: @venue.Score.ToString("F1")
                            </MudText>
                            <MudChip Size="Size.Small" Color="Color.Secondary">
                                @venue.Zone
                            </MudChip>
                            <MudChip Size="Size.Small">@venue.Category</MudChip>
                        </MudCardContent>
                        <MudCardActions>
                            <MudButton Variant="Variant.Text" 
                                       Color="Color.Primary">
                                Ver Detalles
                            </MudButton>
                        </MudCardActions>
                    </MudCard>
                </MudItem>
            }
        </MudGrid>
    }
</MudContainer>

@code {
    // Estado del componente
    private List<VenueDto>? venues;
    private bool loading = true;
    private string searchZone = "";
    private string searchCategory = "";

    // Se ejecuta cuando el componente se monta
    protected override async Task OnInitializedAsync() {
        await LoadVenues();
    }

    // Método para cargar venues desde la API
    private async Task LoadVenues() {
        loading = true;
        try {
            // Construir query string
            var query = $"/api/venues?page=1&pageSize=20";
            if (!string.IsNullOrEmpty(searchZone))
                query += $"&zone={searchZone}";
            if (!string.IsNullOrEmpty(searchCategory))
                query += $"&category={searchCategory}";

            // Llamada HTTP
            venues = await Http.GetFromJsonAsync<List<VenueDto>>(query);
        }
        catch (Exception ex) {
            Console.WriteLine($"Error loading venues: {ex.Message}");
            venues = new List<VenueDto>();
        }
        finally {
            loading = false;
        }
    }
}
```

### Desglose del Código:

1. **`@page "/venues"`**: Define la ruta (http://localhost:5002/venues)
2. **`@inject HttpClient Http`**: Inyecta el servicio HTTP
3. **`@bind-Value="@searchZone"`**: Vincula input con variable C#
4. **`OnClick="LoadVenues"`**: Ejecuta método al hacer clic
5. **`@if (loading)`**: Renderizado condicional
6. **`@foreach (var venue in venues)`**: Bucle sobre la lista
7. **`@venue.Name`**: Muestra propiedad del objeto
8. **`protected override async Task OnInitializedAsync()`**: Carga inicial
9. **`await Http.GetFromJsonAsync<T>(url)`**: Llamada HTTP GET

---

## Mejores Prácticas

### 1. Organización de Código
```razor
<!-- ✅ BIEN: Separar UI y lógica -->
<MudButton OnClick="HandleClick">Clic</MudButton>

@code {
    private void HandleClick() {
        // Lógica aquí
    }
}

<!-- ❌ MAL: Lógica inline compleja -->
<MudButton OnClick="@(() => { var x = 5; var y = 10; Console.WriteLine(x + y); })">
    Clic
</MudButton>
```

### 2. Manejo de Errores
```razor
@code {
    private async Task LoadData() {
        try {
            data = await Http.GetFromJsonAsync<T>("api/endpoint");
        }
        catch (HttpRequestException ex) {
            errorMessage = $"Error de red: {ex.Message}";
        }
        catch (Exception ex) {
            errorMessage = $"Error inesperado: {ex.Message}";
        }
    }
}
```

### 3. Null Safety
```razor
@code {
    private List<Item>? items;  // Nullable
    
    // Opción 1: Null-coalescing
    var count = items?.Count ?? 0;
    
    // Opción 2: Null-conditional
    @if (items != null && items.Any()) {
        // Usar items
    }
    
    // Opción 3: Inicializar con valor por defecto
    private List<Item> items = new();
}
```

### 4. Performance
```razor
<!-- ✅ Usar @key para listas dinámicas -->
@foreach (var item in items) {
    <MudCard @key="item.Id">
        <MudText>@item.Name</MudText>
    </MudCard>
}

<!-- Evitar renderizados innecesarios -->
@code {
    protected override bool ShouldRender() {
        // Solo renderiza si hay cambios relevantes
        return hasChanges;
    }
}
```

---

## Recursos Adicionales

### Documentación Oficial
- **Blazor**: https://learn.microsoft.com/es-es/aspnet/core/blazor/
- **MudBlazor**: https://mudblazor.com/
- **MudBlazor - Galería de Componentes**: https://mudblazor.com/components/list

### Herramientas Online
- **Try MudBlazor**: https://try.mudblazor.com/ (Editor online)
- **Blazor Playground**: https://blazorplayground.com/

### Tutoriales
- **Microsoft Learn - Blazor**: https://learn.microsoft.com/es-es/training/paths/build-web-apps-with-blazor/
- **MudBlazor Templates**: https://mudblazor.com/getting-started/templates

### Extensiones VS Code Recomendadas
- **C# Dev Kit**: Soporte completo para C#
- **Blazor Snippet Pack**: Snippets útiles
- **MudBlazor Snippets**: Snippets de componentes MudBlazor

---

## Comandos Útiles

```powershell
# Ejecutar proyecto
dotnet run

# Compilar sin ejecutar
dotnet build

# Ver errores en tiempo real
dotnet watch run

# Limpiar compilaciones anteriores
dotnet clean

# Restaurar dependencias
dotnet restore
```

---

## Próximos Pasos

Una vez domines estos conceptos, puedes avanzar a:
1. **Componentes personalizados reutilizables**
2. **Estado global con Fluxor** (Redux para Blazor)
3. **Autenticación y autorización**
4. **Progressive Web App (PWA)**
5. **Integración con JavaScript (JS Interop)**
6. **SignalR para tiempo real**

---

**Guía creada para**: Proyecto Guía Gastronómica Justa - Huelva  
**Fecha**: Diciembre 2025  
**Framework**: .NET 8 + Blazor WebAssembly + MudBlazor
