using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Api.Services;
using GuiaGastronomica.Api.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar DbContext con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Semantic Kernel con Ollama usando el conector oficial
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

// Registrar GooglePlacesService
builder.Services.AddScoped<GooglePlacesService>();
builder.Services.AddHttpClient<GooglePlacesService>();

// Registrar ZoneAssignmentService
builder.Services.AddScoped<ZoneAssignmentService>();

// Configurar SignalR para chatbot
builder.Services.AddSignalR();

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

var app = builder.Build();

// Seed database con datos de prueba
// DESHABILITADO: Las migraciones y seeding se hacen manualmente vía endpoints
// Para habilitar de nuevo, descomentar las líneas de abajo
/*
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    DataSeeder.SeedAsync(context).Wait();
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

app.Run();
