# Guía Gastronómica Justa — Documento de diseño v1.0

**Fecha**: 2025-12-09  
**Autor**: CiGaRReTeM  
**Versión**: 1.0

---

## 1. Resumen ejecutivo

Crear una plataforma web para mostrar rankings justos y transparentes de bares y restaurantes en la ciudad de Huelva. El objetivo es ofrecer un ranking basado en señales reales de calidad (no sesgado por publicidad o pagos a plataformas) y dar visibilidad a los establecimientos que realmente lo merecen.

**Principio fundamental**: **100% componentes gratuitos y open-source** para desarrollo y operación local. El único coste será el despliegue en producción (hosting, dominio).

---

## 2. Objetivos principales

1. **Ranking justo**: clasificar locales según señales reales de calidad (reseñas verificadas, fotos geolocalizadas, precios actualizados, tráfico, opiniones de food bloggers locales).
2. **Transparencia**: evitar el "pay-to-top" y minimizar manipulaciones mediante procesos de verificación y señales múltiples.
3. **Experiencia de usuario**: proveer búsqueda, filtros, rankings por zona y chatbot conversacional con IA.
4. **Feedback comunitario**: permitir a usuarios aportar datos, verificar información y recalcular rankings en tiempo real.

---

## 3. Público objetivo

- **Residentes de Huelva**: buscan recomendaciones fiables y locales auténticos.
- **Foodies y visitantes**: quieren descubrir locales con mérito real sin sesgos publicitarios.
- **Propietarios de bares/restaurantes**: buscan feedback honesto y transparencia en el scoring.

---

## 4. MVP (versión mínima viable)

### Funcionalidades core

1. **Lista de locales** con fichas básicas: nombre, dirección, fotos, puntuación calculada, horarios, precios.
2. **Fuentes de datos iniciales**:
   - Reseñas de usuarios (subidas manualmente).
   - Scraping básico de webs públicas y agregación de reseñas.
   - OpenStreetMap (OSM) para datos geográficos.
3. **Motor de scoring simple**: combina número de reseñas verificadas, sentimiento medio, fotos geolocalizadas, menciones en blogs locales.
4. **Interfaz Blazor** con búsqueda y filtros (zona, tipo de cocina, rango de precio).
5. **Panel administrativo**: validar reseñas, moderar contenido, gestionar denuncias.
6. **Chatbot conversacional (IA integrada)**: permite a usuarios interactuar con IA para dar feedback, verificar información y aportar datos nuevos que se ingesten y recalculen rankings automáticamente.

### Roadmap post-MVP

- **Mapa interactivo por zonas**: vista de mapa (Leaflet.js o MapLibre GL) con delimitación de barrios/distritos; rankings globales + por zona; geocodificación con Nominatim (OSM).
- **PWA (Progressive Web App)**: instalación en móvil, notificaciones push.
- **Búsqueda semántica avanzada**: preguntas en lenguaje natural ("mejores tapas veganas en el centro").
- **Recomendaciones personalizadas**: perfiles de usuario con preferencias y sugerencias ML.
- **API pública REST**: consumo por terceros (con rate limiting).
- **Internacionalización (i18n)**: soporte español + inglés.
- **Gamificación**: recompensas por reseñas útiles, insignias para usuarios activos.

---

## 5. Fuentes de datos

### 5.1. Fuentes gratuitas prioritarias

- **OpenStreetMap (OSM)**: datos geográficos, direcciones, categorías (100% gratuito, open data).
- **Nominatim (OSM)**: geocodificación (conversión dirección → coordenadas).
- **Webs municipales y turismo oficial de Huelva**: listados oficiales, certificaciones, eventos gastronómicos.
- **Food bloggers y medios locales**: scraping de blogs, newsletters, prensa digital (HuelvaInformación, etc.).
- **Wikimedia Commons**: fotos de lugares públicos.
- **Flickr / Unsplash**: fotos públicas con EXIF geolocalizado.

### 5.2. APIs gratuitas con límites (opcionales)

- **Google Maps / Google Places API**: créditos gratuitos mensuales (~$200/mes); reseñas, valoraciones, horarios, fotos.
- **Yelp API**: API gratuita con límites; reseñas, fotos, valoraciones.
- **Foursquare API**: check-ins históricos (API disponible con tier gratuito).

### 5.3. Redes sociales (señales indirectas)

- **Instagram**: hashtags (`#HuelvaCome`, `#GastroHuelva`, `#RestaurantesHuelva`), geotags, engagement.
- **X/Twitter**: búsquedas por palabras clave y geolocalización.
- **Facebook**: posts públicos, check-ins, valoraciones (Graph API con límites).
- **TikTok**: vídeos con hashtags locales (scraping complejo, útil para tendencias).

### 5.4. Crowdsourcing y comunidad

- **Formulario de reseñas verificadas**: usuarios locales aportan reseñas (con verificación por email o cuenta social).
- **Sistema de votos/upvotes**: detectar manipulación mediante análisis de patrones.
- **Chatbot conversacional**: captar feedback estructurado en tiempo real (ver sección dedicada).

**Nota importante**: respetar términos de servicio (ToS), robots.txt y políticas de privacidad; preferir APIs oficiales y fuentes con permiso explícito. Implementar rate limiting y caching para evitar bloqueos.

---

## 6. Stack tecnológico (100% gratuito para desarrollo)

### 6.1. Lenguajes y frameworks

- **Backend**: ASP.NET Core 8+ (C#) — Web API REST.
- **Frontend**: Blazor WebAssembly (recomendado para PWA y SEO con prerendering) o Blazor Server.
- **UI Components**: MudBlazor o Radzen Blazor Components (bibliotecas gratuitas, responsive).

### 6.2. Base de datos

- **Relacional**: 
  - **Desarrollo/prototipo**: SQLite (sin servidor, archivo local).
  - **Producción**: PostgreSQL (gratuito, autohospedado en VPS o Supabase free tier).
- **Tablas principales**: `Venues`, `Reviews`, `Users`, `DataSources`, `Zones`, `ScoringMetrics`, `UserFeedback`, `ChatHistory`.

### 6.3. Vector DB (RAG)

- **Prototipo**: in-memory (Microsoft.SemanticKernel.Memory).
- **Producción**: **Qdrant** (Docker, autohospedado, 100% gratuito) o **pgvector** (extensión PostgreSQL, gratuita).

### 6.4. IA y LLMs

- **Orquestación**: Semantic Kernel (.NET) para prompts, embeddings y pipeline RAG.
- **Modelos LLM (prioridad gratuita)**:
  - **Ollama** (local, gratuito): `llama3.2`, `mistral`, `phi3` para chatbot y análisis de sentimiento.
  - **llama.cpp / ggml**: modelos locales para embeddings.
  - Opción futura: APIs comerciales (OpenAI, Anthropic) si necesario, pero no en MVP.
- **OCR para fotos**: **Tesseract** (gratuito, local) para extraer precios/platos de menús.
- **NLP**: **ML.NET** (.NET) o **spaCy** (Python, integrable vía API local) para extracción de entidades.

### 6.5. Mapas y geocodificación

- **Mapas interactivos**: Leaflet.js o MapLibre GL (ambos open-source).
- **Datos geográficos**: OpenStreetMap (OSM).
- **Geocodificación**: Nominatim (OSM, gratuito).

### 6.6. Background jobs y caché

- **Jobs**: Hangfire (dashboard web incluido, gratuito) o Quartz.NET (gratuito).
- **Caché**: Redis (gratuito, autohospedado con Docker).

### 6.7. Logs y monitoreo

- **Logs**: Serilog (gratuito) con salida a consola/archivo o Seq (gratuito para desarrollo).
- **Métricas**: Prometheus + Grafana (ambos gratuitos, autohospedados).

### 6.8. CI/CD

- **GitHub Actions**: build, test, deploy (gratuito para repos públicos y privados con límites generosos).

### 6.9. Hosting (único coste del proyecto)

- **Prototipo/desarrollo**: Docker Compose local (100% gratuito).
- **Producción (con coste)**:
  - VPS (DigitalOcean, Hetzner, OVH): ~5-10 €/mes.
  - Railway / Render: free tiers con limitaciones (posible para empezar).
  - Supabase: PostgreSQL managed, free tier generoso (hasta 500 MB, 2 CPU).
- **Almacenamiento de fotos**:
  - Desarrollo: local (`wwwroot/uploads`).
  - Producción: MinIO (S3-compatible, autohospedado, gratuito) o carpeta en VPS.
- **CDN (opcional)**: Cloudflare (free tier para assets estáticos).

---

## 7. Arquitectura técnica

### 7.1. Frontend (Blazor)

- **Blazor WebAssembly** con prerendering para SEO.
- Componentes Razor reutilizables: fichas de locales, lista, mapa, filtros, chat flotante.
- **JS Interop**: integración con Leaflet.js/MapLibre GL para mapas.
- **SignalR**: comunicación en tiempo real con backend (chatbot).

### 7.2. Backend (ASP.NET Core Web API)

#### Endpoints REST principales

- `GET /api/venues` — lista de locales (con filtros: zona, categoría, precio).
- `GET /api/venues/{id}` — ficha detallada de local.
- `GET /api/rankings` — ranking global y por zona.
- `POST /api/reviews` — enviar reseña de usuario.
- `GET /api/admin/reviews` — panel moderación (autenticado).

#### SignalR Hub

- `ChatHub` — WebSocket para comunicación en tiempo real con chatbot IA.

#### Servicios internos

1. **Servicio de ingestión**: colecciona datos de fuentes configuradas (APIs, scraping, RSS), normaliza entradas; ejecución periódica vía background jobs (Hangfire).
2. **Servicio de scoring**: calcula puntuaciones y justificaciones (explicables) basadas en señales múltiples; algoritmo transparente y auditable.
3. **Servicio RAG + Semantic Kernel**: indexa textos y documentos, gestiona embeddings (Ollama local), responde preguntas mediante búsqueda semántica.
4. **Servicio de chatbot conversacional**: orquesta interacción usuario-IA, extrae intenciones y entidades (LLM local), valida y almacena feedback estructurado.
5. **Servicio de geocodificación**: convierte direcciones en coordenadas (Nominatim OSM).
6. **Servicio de análisis de sentimiento**: clasifica reseñas usando LLM local (Ollama).

### 7.3. Almacenamiento

- **Base relacional**: PostgreSQL (producción) o SQLite (prototipo).
- **Vector DB**: Qdrant (Docker) o pgvector (PostgreSQL).
- **Fotos**: local (desarrollo) o MinIO/VPS (producción).

### 7.4. Background jobs

- Ingestión periódica de datos (cada X horas).
- Recalculo de rankings (diario o cuando se acumulan Y feedback nuevos).
- Limpieza de datos antiguos.

### 7.5. Seguridad y autenticación

- **ASP.NET Core Identity**: gestión de usuarios y roles (admin, moderador, usuario).
- **JWT tokens**: autenticación en API.
- **Rate limiting**: endpoints públicos para evitar abuso.
- **HTTPS**: obligatorio en producción.

---

## 8. Chatbot conversacional (IA integrada)

### 8.1. Objetivo

Permitir a usuarios interactuar con un chatbot basado en IA para:
1. **Dar feedback y opiniones** sobre locales de forma conversacional.
2. **Verificar información** existente ("¿Este restaurante sigue abierto?", "¿Los precios son correctos?").
3. **Aportar datos nuevos** (horarios actualizados, nuevos platos, fotos recientes).
4. **Resolver dudas** sobre el ranking y el scoring (transparencia).
5. **Ingestar automáticamente** la información capturada y recalcular rankings.

### 8.2. Casos de uso

- **Usuario**: "He estado en Bar Pepe y la tortilla estaba buenísima, pero el servicio fue lento."
  - **IA**: extrae sentimiento (positivo comida, negativo servicio), solicita detalles (fecha, precio), guarda feedback estructurado.

- **Usuario**: "El restaurante La Marinera cerró hace 2 meses."
  - **IA**: marca local como "cerrado temporalmente", solicita confirmación adicional (fotos, enlace noticia).

- **Usuario**: "¿Por qué El Rincón Andaluz está en el puesto 15?"
  - **IA**: explica algoritmo de scoring (señales múltiples, no solo reseñas), muestra factores del ranking.

- **Usuario**: "Acabo de visitar Casa Antonio, puedo subir fotos del menú actualizado."
  - **IA**: solicita fotos, las procesa con OCR (Tesseract), extrae precios/platos, actualiza ficha.

### 8.3. Arquitectura técnica

#### Frontend (Blazor)

- **Componente chat flotante**: widget esquina inferior derecha (estilo chat soporte).
- **Interfaz conversacional**: burbuja mensajes, input texto, adjuntar fotos.
- **SignalR**: comunicación en tiempo real con backend.

#### Backend (ASP.NET Core)

- **SignalR Hub** (`ChatHub`): conexión persistente con frontend.
- **Servicio de chatbot (Semantic Kernel)**:
  - Recibe mensaje del usuario.
  - Enriquece contexto con RAG (busca info relevante en vector DB).
  - Invoca LLM (Ollama local: `llama3.2`, `mistral`, `phi3`) con prompt estructurado:
    - Rol: asistente de Guía Gastronómica Justa.
    - Contexto: info del local (obtenida vía RAG).
    - Tarea: extraer intenciones (feedback, verificación, aporte datos, pregunta).
  - **Extracción de entidades estructuradas**: mediante funciones Semantic Kernel o prompts JSON:
    - Nombre del local.
    - Tipo de feedback (comida, servicio, precio, ambiente).
    - Sentimiento (positivo, negativo, neutro).
    - Datos nuevos (horarios, precios, platos, estado).
  - **Almacena en DB**: tabla `UserFeedback` (`UserId`, `VenueId`, `Message`, `ExtractedData` JSON, `Sentiment`, `Verified`, `CreatedAt`).
  - **Respuesta al usuario**: confirma recepción, solicita aclaraciones, agradece aporte.

- **Servicio de validación y moderación**:
  - Detecta spam o feedback malicioso (análisis de patrones, rate limiting por usuario).
  - Requiere confirmación múltiple para cambios críticos (ej.: cerrar local).
  - Moderadores humanos revisan feedback marcado como sospechoso.

- **Servicio de recalculo de scoring**:
  - Ejecuta periódicamente (cada X horas o cuando se acumula Y feedback).
  - Integra datos extraídos del chatbot en scoring:
    - Incrementa peso de locales con feedback reciente positivo.
    - Penaliza locales con múltiples reportes de cierre/problemas.
    - Ajusta precios/horarios según aportes verificados.

### 8.4. Flujo de ingesta y recalculo

1. Usuario envía mensaje al chatbot.
2. IA extrae intenciones y entidades (local, sentimiento, datos).
3. Backend guarda en `UserFeedback` (estado: `pending_verification`).
4. Si es dato crítico (cierre, cambio drástico), requiere confirmación adicional.
5. Moderador o sistema automático valida (marca como `verified`).
6. Servicio de recalculo integra datos verificados en scoring y actualiza rankings.
7. Usuario recibe notificación de aporte aceptado (opcional: gamificación con puntos).

### 8.5. Tecnologías (100% gratuitas)

- **Semantic Kernel (.NET)**: orquestación LLM, prompts, funciones (plugins).
- **SignalR**: comunicación tiempo real frontend-backend.
- **Ollama (local)**: modelos gratuitos (`llama3.2`, `mistral`, `phi3`).
- **Tesseract OCR**: extracción texto de fotos (gratuito, local).
- **ML.NET**: extracción entidades y análisis sentimiento (gratuito, .NET nativo).

### 8.6. Privacidad y moderación

- **Anonimización opcional**: permitir feedback anónimo (menor peso en scoring).
- **Registro conversaciones**: historial chat para auditoría y mejora modelo.
- **Detección manipulación**: analizar patrones usuarios con feedback masivo (campañas pagadas).
- **Transparencia**: informar usuarios que IA puede cometer errores y hay moderación humana.

### 8.7. Métricas de éxito del chatbot

- Nº de interacciones diarias.
- Tasa de extracción correcta de entidades (precisión NLP).
- Nº de aportes verificados e integrados en sistema.
- Satisfacción de usuarios (encuesta post-chat opcional).
- Reducción de datos desactualizados (locales cerrados detectados rápidamente).

---

## 9. Privacidad, legal y moderación

- **GDPR**: verificar que scraping y recolección de datos cumplen ToS y legislación local.
- **Moderación**: implementar verificación de reseñas para evitar spam y pagos fraudulentos.
- **Reclamaciones**: proveer mecanismo para que locales puedan reclamar datos incorrectos.
- **Transparencia del scoring**: documentar algoritmo de puntuación y permitir a locales ver qué señales afectan su ranking.

---

## 10. Métricas de éxito (KPI)

- Nº de locales indexados y validados.
- Nº de reseñas verificadas vs no verificadas.
- Tasa de aceptación de reseñas (moderación).
- Tráfico y uso (visitas únicas, búsquedas realizadas).
- Interacciones diarias con chatbot.
- Aportes de usuarios verificados e integrados.

---

## 11. Consideraciones de calidad y escalabilidad

### 11.1. Calidad y confiabilidad de datos

- **Verificación de fuentes**: priorizar fuentes oficiales (OSM, APIs con TOS claros) sobre scraping no autorizado.
- **Deduplicación**: detectar y fusionar entradas duplicadas (mismo local con nombres ligeramente distintos).
- **Detección de anomalías**: identificar reseñas sospechosas (patrones de spam, cuentas bot, campañas pagadas).

### 11.2. Escalabilidad

- **Caché**: Redis para cachear rankings, búsquedas frecuentes y fichas de locales populares.
- **Paginación y filtros**: limitar resultados por página (20-50) y ofrecer filtros granulares.
- **Índices DB**: crear índices en columnas frecuentes (`zone`, `category`, `score`, `created_at`).
- **CDN**: servir assets estáticos (fotos, JS, CSS) desde CDN (Cloudflare free tier).

---

## 12. Próximos pasos

1. ✅ **MVP y arquitectura definidos** — Hecho: mapa por zonas en roadmap, fuentes ampliadas, chatbot con IA, arquitectura detallada.
2. **Decisiones de stack**:
   - Blazor WebAssembly (recomendado) o Blazor Server.
   - Vector DB: empezar con in-memory, migrar a Qdrant cuando escale.
   - Modelo LLM: Ollama local (`llama3.2` o `mistral`) para prototipo.
3. **Crear scaffold mínimo**: backend ASP.NET Core + Blazor WASM + modelos compartidos + configuración inicial Semantic Kernel.
4. **Implementar pipeline de ingestión**: 2-3 fuentes iniciales (OSM, scraping blogs locales, formulario reseñas).
5. **Prototipar RAG y chatbot**: Semantic Kernel + Ollama + embeddings in-memory.
6. **Desplegar localmente**: Docker Compose con PostgreSQL + Redis + Qdrant.
7. **Testing y validación**: probar con datos reales de Huelva.
8. **Despliegue en producción**: VPS (único coste) con Docker Compose + dominio + SSL (Let's Encrypt gratuito).

---

## 13. Resumen de componentes gratuitos

| Componente | Herramienta | Coste |
|------------|-------------|-------|
| Backend | ASP.NET Core 8+ (C#) | Gratuito |
| Frontend | Blazor WebAssembly | Gratuito |
| UI Components | MudBlazor / Radzen Blazor | Gratuito |
| Base de datos | PostgreSQL / SQLite | Gratuito |
| Vector DB | Qdrant (Docker) / pgvector | Gratuito |
| LLM | Ollama (`llama3.2`, `mistral`, `phi3`) | Gratuito |
| IA Orchestration | Semantic Kernel (.NET) | Gratuito |
| OCR | Tesseract | Gratuito |
| NLP | ML.NET / spaCy | Gratuito |
| Mapas | Leaflet.js / MapLibre GL | Gratuito |
| Datos geográficos | OpenStreetMap (OSM) | Gratuito |
| Geocodificación | Nominatim (OSM) | Gratuito |
| Background Jobs | Hangfire / Quartz.NET | Gratuito |
| Caché | Redis (Docker) | Gratuito |
| Logs | Serilog + Seq (desarrollo) | Gratuito |
| Monitoreo | Prometheus + Grafana | Gratuito |
| CI/CD | GitHub Actions | Gratuito (con límites) |
| Almacenamiento fotos | MinIO (Docker) | Gratuito |
| CDN | Cloudflare (free tier) | Gratuito |
| **Hosting producción** | **VPS (DigitalOcean, Hetzner)** | **5-10 €/mes** ⚠️ |

**Total coste desarrollo**: 0 €  
**Total coste producción**: ~5-10 €/mes (VPS + dominio)

---

## 14. Sistema de Feedback y Aprendizaje Continuo (Pendiente)

### Descripción general
Sistema de retroalimentación en tiempo real que permite al chatbot aprender de las interacciones de los usuarios y personalizar recomendaciones basándose en preferencias detectadas.

### Componentes principales

#### 14.1. Plugins de Semantic Kernel (Funciones Nativas)
Funciones que el LLM puede llamar automáticamente durante la conversación:

- **`buscar_restaurantes`**: Búsqueda dinámica según criterios (tipo cocina, zona, precio)
- **`obtener_opiniones`**: Consulta reviews en tiempo real de un venue
- **`registrar_feedback`**: Guarda thumbs up/down y ratings del usuario
- **`registrar_preferencia`**: Almacena preferencias explícitas del usuario
- **`obtener_perfil_usuario`**: Recupera preferencias aprendidas de sesiones anteriores
- **`calcular_recomendacion_personalizada`**: Ordena venues según perfil del usuario

#### 14.2. Base de datos de feedback
Nuevas tablas en SQLite:

```sql
-- Feedback explícito de usuarios
CREATE TABLE ChatFeedback (
    Id INTEGER PRIMARY KEY,
    SessionId TEXT NOT NULL,
    UserMessage TEXT,
    BotResponse TEXT,
    VenueRecommended INTEGER,
    Rating INTEGER,         -- 1-5 estrellas (opcional)
    IsHelpful BOOLEAN,      -- thumbs up/down
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (VenueRecommended) REFERENCES Venues(Id)
);

-- Preferencias aprendidas por usuario/sesión
CREATE TABLE UserPreferences (
    Id INTEGER PRIMARY KEY,
    SessionId TEXT NOT NULL,
    PreferenceType TEXT NOT NULL,  -- "cuisine", "zone", "price", "ambiance"
    PreferenceValue TEXT NOT NULL,
    Confidence REAL DEFAULT 0.5,   -- 0.0-1.0 (aumenta con interacciones)
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Índices para consultas rápidas
CREATE INDEX idx_feedback_session ON ChatFeedback(SessionId);
CREATE INDEX idx_preferences_session ON UserPreferences(SessionId);
CREATE INDEX idx_feedback_venue ON ChatFeedback(VenueRecommended);
```

#### 14.3. Servicios backend
Nuevos servicios en `GuiaGastronomica.Api/Services/`:

- **`FeedbackService.cs`**: CRUD de feedback y estadísticas
- **`PreferenceAnalyzer.cs`**: Detecta patrones (cocina favorita, zona preferida, rango de precio)
- **`RecommendationEngine.cs`**: Ordena venues según perfil del usuario
- **`VectorMemoryService.cs`** (avanzado): Embeddings de preferencias con Qdrant/pgvector

#### 14.4. Frontend (Blazor)
Componente `FeedbackButtons.razor`:

```razor
@if (message.ContainsVenueRecommendation)
{
    <div class="feedback-buttons">
        <MudIconButton Icon="@Icons.Material.Filled.ThumbUp" 
                       Color="Color.Success"
                       OnClick="@(() => SendFeedback(message.VenueId, true))" />
        <MudIconButton Icon="@Icons.Material.Filled.ThumbDown" 
                       Color="Color.Error"
                       OnClick="@(() => SendFeedback(message.VenueId, false))" />
        <MudRating @bind-SelectedValue="rating" 
                   OnClick="@(() => SendRating(message.VenueId, rating))" />
    </div>
}
```

#### 14.5. Integración con ChatService
Modificar `BuildVenuesContext()` para incluir preferencias:

```csharp
public async Task<string> ProcessMessageAsync(string userMessage, string sessionId)
{
    // 1. Analizar preferencias del usuario
    var userPrefs = await _preferenceAnalyzer.AnalyzeAsync(sessionId);
    
    // 2. Obtener venues personalizados
    var venues = await _recommendationEngine.GetRecommendedAsync(userPrefs);
    
    // 3. Construir contexto enriquecido
    var contextMessage = $@"
Contexto: Eres un asistente experto en gastronomía de Huelva.

PREFERENCIAS APRENDIDAS DEL USUARIO:
{(userPrefs.FavoriteCuisine != null ? $"- Prefiere cocina: {userPrefs.FavoriteCuisine}" : "")}
{(userPrefs.FavoriteZone != null ? $"- Zona preferida: {userPrefs.FavoriteZone}" : "")}
{(userPrefs.PreferredPriceRange.HasValue ? $"- Rango de precio: {userPrefs.PreferredPriceRange}" : "")}

RESTAURANTES DISPONIBLES (ordenados por relevancia):
{BuildVenuesContext(venues)}";
    
    // ... resto del código
}
```

### Roadmap de implementación

#### Fase 1: Feedback Básico (1-2 días) ✅ Prioritario
1. Crear modelos `ChatFeedback` y `UserPreferences`
2. Migración de base de datos (nuevas tablas)
3. Implementar `FeedbackService.cs`
4. Plugin SK `registrar_feedback`
5. Botones thumbs up/down en frontend
6. Dashboard básico de métricas de feedback

#### Fase 2: Detección de Preferencias (3-4 días)
1. Implementar `PreferenceAnalyzer.cs`
2. Plugin SK `obtener_perfil_usuario`
3. Modificar `ChatService` para incluir preferencias en contexto
4. Testing con múltiples sesiones simuladas

#### Fase 3: Recomendaciones Personalizadas (5-7 días)
1. Implementar `RecommendationEngine.cs` con algoritmo de scoring
2. Plugin SK `calcular_recomendacion_personalizada`
3. A/B testing de prompts personalizados vs genéricos
4. Métricas de efectividad (% thumbs up antes/después)

#### Fase 4: RAG + Vector Memory (avanzado, 2-3 semanas)
1. Integrar Qdrant o pgvector para embeddings
2. Implementar `VectorMemoryService.cs`
3. Guardar interacciones como embeddings
4. Búsqueda semántica de preferencias similares
5. Memoria a largo plazo entre sesiones

### Beneficios esperados
- ✅ Recomendaciones personalizadas por usuario
- ✅ Mejora continua del chatbot con datos reales
- ✅ Detección automática de patterns (usuarios de tapas, alta cocina, etc.)
- ✅ Métricas de calidad de recomendaciones
- ✅ Base para sistema de recomendación híbrido (LLM + ML tradicional)

### Dependencias
- Semantic Kernel 1.30.0+ (plugins nativos)
- SQLite (tablas de feedback)
- Opcional: Qdrant/pgvector para embeddings (Fase 4)

### Estado
⏳ **Pendiente de implementación** — Documentado el 2025-12-15

---

**Fin del documento v1.0** ✅
