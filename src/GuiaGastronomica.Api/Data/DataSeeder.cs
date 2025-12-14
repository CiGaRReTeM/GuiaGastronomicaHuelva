using GuiaGastronomica.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GuiaGastronomica.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Verificar si ya hay datos
        if (await context.Venues.AnyAsync())
        {
            return; // Ya hay datos, no hacer nada
        }

        // 1. Crear Zonas
        var zonas = new List<Zone>
        {
            new Zone { Id = 1, Name = "Centro" },
            new Zone { Id = 2, Name = "Molinos" },
            new Zone { Id = 3, Name = "Isla Cristina" },
            new Zone { Id = 4, Name = "Punta Umbría" },
            new Zone { Id = 5, Name = "El Rompido" }
        };

        context.Zones.AddRange(zonas);
        await context.SaveChangesAsync();

        // 2. Crear Venues de ejemplo
        var venues = new List<Venue>
        {
            // Centro de Huelva
            new Venue
            {
                Name = "Restaurante Azabache",
                Address = "Calle Vázquez López, 22, Huelva",
                Zone = "Centro",
                Category = "Restaurante",
                Description = "Cocina mediterránea con productos locales de calidad",
                Score = 8.7,
                Latitude = 37.2574,
                Longitude = -6.9501,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Venue
            {
                Name = "Bar El Rinconcito",
                Address = "Plaza de las Monjas, 5, Huelva",
                Zone = "Centro",
                Category = "Bar",
                Description = "Tapas tradicionales y ambiente familiar",
                Score = 7.9,
                Latitude = 37.2568,
                Longitude = -6.9495,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Venue
            {
                Name = "Mesón Don Jamón",
                Address = "Calle Rico, 8, Huelva",
                Zone = "Centro",
                Category = "Mesón",
                Description = "Especialidad en jamón ibérico y carnes",
                Score = 8.3,
                Latitude = 37.2580,
                Longitude = -6.9510,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Isla Cristina
            new Venue
            {
                Name = "Restaurante Casa Rufino",
                Address = "Avenida de la Playa, 15, Isla Cristina",
                Zone = "Isla Cristina",
                Category = "Restaurante",
                Description = "Pescado fresco y mariscos de la zona",
                Score = 9.1,
                Latitude = 37.1986,
                Longitude = -7.3183,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Venue
            {
                Name = "Bar La Marina",
                Address = "Puerto Deportivo, s/n, Isla Cristina",
                Zone = "Isla Cristina",
                Category = "Bar",
                Description = "Vistas al puerto, tapas de pescado",
                Score = 8.0,
                Latitude = 37.2020,
                Longitude = -7.3200,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Punta Umbría
            new Venue
            {
                Name = "Chiringuito El Paraíso",
                Address = "Playa de Punta Umbría, Punta Umbría",
                Zone = "Punta Umbría",
                Category = "Chiringuito",
                Description = "Espetos en la playa, ambiente playero",
                Score = 8.5,
                Latitude = 37.1800,
                Longitude = -6.9670,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Venue
            {
                Name = "Restaurante Los Marineros",
                Address = "Avenida del Océano, 32, Punta Umbría",
                Zone = "Punta Umbría",
                Category = "Restaurante",
                Description = "Cocina marinera tradicional",
                Score = 8.8,
                Latitude = 37.1820,
                Longitude = -6.9680,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // El Rompido
            new Venue
            {
                Name = "Restaurante Jamón y Gambas",
                Address = "Calle El Rompido, 10, Cartaya",
                Zone = "El Rompido",
                Category = "Restaurante",
                Description = "Fusión de sierra y mar",
                Score = 8.6,
                Latitude = 37.2150,
                Longitude = -7.1350,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Barrio Molinos
            new Venue
            {
                Name = "Taberna El Molinero",
                Address = "Calle San José, 45, Huelva",
                Zone = "Molinos",
                Category = "Taberna",
                Description = "Cocina casera y ambiente acogedor",
                Score = 7.8,
                Latitude = 37.2620,
                Longitude = -6.9450,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Venue
            {
                Name = "Cervecería La Espiga",
                Address = "Avenida de Alemania, 20, Huelva",
                Zone = "Molinos",
                Category = "Cervecería",
                Description = "Variedad de cervezas artesanales y tapas",
                Score = 8.2,
                Latitude = 37.2630,
                Longitude = -6.9460,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Venues.AddRange(venues);
        await context.SaveChangesAsync();

        // 3. Crear Reviews de ejemplo
        var reviews = new List<Review>
        {
            new Review
            {
                VenueId = 1, // Azabache
                Rating = 9,
                Content = "Excelente comida, el atún rojo espectacular",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Review
            {
                VenueId = 1,
                Rating = 8,
                Content = "Buena relación calidad-precio",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Review
            {
                VenueId = 4, // Casa Rufino
                Rating = 10,
                Content = "El mejor pescado de Isla Cristina, sin duda",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Review
            {
                VenueId = 4,
                Rating = 9,
                Content = "Langostinos fresquísimos, altamente recomendable",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Review
            {
                VenueId = 6, // El Paraíso
                Rating = 8,
                Content = "Perfecto para comer en la playa",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Review
            {
                VenueId = 7, // Los Marineros
                Rating = 9,
                Content = "Arroz caldoso buenísimo",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        context.Reviews.AddRange(reviews);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Datos de prueba creados: {venues.Count} venues, {reviews.Count} reviews");
    }
}
