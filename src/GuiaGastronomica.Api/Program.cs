using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Api.Services;
using GuiaGastronomica.Api.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Serilog;

Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("🚀 Iniciando Guía Gastronómica Justa API...");
Console.WriteLine("═══════════════════════════════════════════════════════════");

var builder = WebApplication.CreateBuilder(args);

// Configurar puerto 5001 explícitamente
builder.WebHost.UseUrls("http://localhost:5001");

Console.WriteLine("✓ WebApplicationBuilder creado");

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();
Console.WriteLine("✓ Serilog configurado");

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("✓ Servicios Swagger y Controladores agregados");

// Configurar DbContext con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

Console.WriteLine("✓ DbContext SQLite configurado");

// Configurar Semantic Kernel con Ollama usando el conector oficial
// NOTA: Ollama debe estar ejecutándose en http://localhost:11434
var kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0070
kernelBuilder.AddOllamaChatCompletion(
    modelId: "llama3.2:3b",
    endpoint: new Uri("http://localhost:11434")
);
#pragma warning restore SKEXP0070

var kernel = kernelBuilder.Build();
builder.Services.AddSingleton(kernel);

// Registrar ChatService
builder.Services.AddScoped<ChatService>();

// Configurar SignalR para chatbot
builder.Services.AddSignalR();

Console.WriteLine("✓ ChatService y SignalR configurados");

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5002", "https://localhost:5003")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

Console.WriteLine("✓ CORS configurado para Blazor Client");

var app = builder.Build();

Console.WriteLine("✓ WebApplication construida");

// Seed database con datos de prueba (comentado - la base de datos ya tiene datos)
// Descomentar solo si necesitas reinicializar la base de datos
/*
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        DataSeeder.SeedAsync(context).Wait();
        Console.WriteLine("✓ Base de datos inicializada con datos de prueba");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Error al inicializar base de datos: {ex.Message}");
}
*/

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Comentar UseHttpsRedirection para permitir conexiones HTTP de SignalR en desarrollo
// app.UseHttpsRedirection();

app.UseCors("AllowBlazorClient");

app.UseAuthorization();

app.MapControllers();

// Mapear Hub de SignalR con RequireCors
app.MapHub<ChatHub>("/chathub").RequireCors("AllowBlazorClient");

// Endpoint de ejemplo
app.MapGet("/", () => "Guía Gastronómica Justa API - v1.0");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

Console.WriteLine("");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("✅ API INICIADA CORRECTAMENTE");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("📍 Endpoints:");
Console.WriteLine("   - Principal: http://localhost:5001");
Console.WriteLine("   - Swagger:   http://localhost:5001/swagger");
Console.WriteLine("   - Health:    http://localhost:5001/health");
Console.WriteLine("═══════════════════════════════════════════════════════════");
Console.WriteLine("");

app.Run();
