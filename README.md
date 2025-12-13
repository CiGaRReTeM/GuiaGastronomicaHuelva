# Guía Gastronómica Justa

**Rankings justos y transparentes de bares y restaurantes en Huelva**

---

## 📋 Descripción

Plataforma web para mostrar rankings de locales gastronómicos en Huelva basados en señales reales de calidad, sin sesgos publicitarios ni "pay-to-top". Combina reseñas verificadas, análisis de sentimiento con IA, geolocalización, y feedback comunitario en tiempo real mediante chatbot conversacional.

**Principio fundamental**: 100% componentes gratuitos y open-source para desarrollo. Único coste: hosting en producción (~5-10 €/mes).

---

## 🎯 Características principales

- ✅ **Ranking justo**: algoritmo transparente basado en señales múltiples (no solo reseñas).
- 🤖 **Chatbot con IA**: interacción conversacional para captar feedback, verificar datos y actualizar rankings.
- 🗺️ **Mapa interactivo** (roadmap): rankings por zonas/barrios con Leaflet.js + OSM.
- 🔍 **Búsqueda y filtros**: por zona, tipo de cocina, rango de precio.
- 📊 **Panel admin**: moderación de reseñas, gestión de denuncias.
- 🌐 **PWA** (roadmap): instalación en móvil, notificaciones push.

---

## 🛠️ Stack tecnológico (100% gratuito)

### Backend
- **ASP.NET Core 8+** (C#) — Web API REST
- **Semantic Kernel** — orquestación de LLMs y RAG
- **Ollama** — LLMs locales gratuitos (`llama3.2`, `mistral`, `phi3`)
- **PostgreSQL** / SQLite — base de datos relacional
- **Qdrant** / pgvector — vector DB para RAG
- **Hangfire** — background jobs

### Frontend
- **Blazor WebAssembly** — UI interactiva (PWA-ready)
- **MudBlazor** / Radzen — componentes UI gratuitos
- **Leaflet.js** / MapLibre GL — mapas interactivos
- **SignalR** — comunicación tiempo real (chatbot)

### IA y datos
- **OpenStreetMap (OSM)** — datos geográficos gratuitos
- **Nominatim** — geocodificación gratuita
- **Tesseract OCR** — extracción texto de fotos
- **ML.NET** — NLP y análisis sentimiento

### Infraestructura
- **Docker Compose** — despliegue local
- **Redis** — caché
- **GitHub Actions** — CI/CD
- **Let's Encrypt** — SSL gratuito

---

## 📖 Documentación

Lee el documento de diseño completo: **[GUIDE.md](./GUIDE.md)**

Incluye:
- MVP y roadmap detallado
- Fuentes de datos (APIs, scraping, crowdsourcing)
- Arquitectura técnica (frontend, backend, IA)
- Chatbot conversacional con IA (casos de uso, flujo)
- Privacidad, legal y moderación
- Tabla resumen de componentes gratuitos

---

## 🚀 Próximos pasos

1. ✅ **Diseño y arquitectura definidos** — Ver [GUIDE.md](./GUIDE.md)
2. 🔨 **Crear scaffold .NET** — API + Blazor WASM + modelos compartidos
3. 🤖 **Prototipar chatbot** — Semantic Kernel + Ollama + RAG
4. 📥 **Pipeline de ingestión** — OSM + scraping blogs + formulario reseñas
5. 🧪 **Testing con datos reales** — Locales de Huelva
6. 🌐 **Despliegue en VPS** — Docker Compose + dominio + SSL

---

## 💡 Cómo contribuir

1. Haz fork del repositorio
2. Crea una rama: `git checkout -b feature/nueva-funcionalidad`
3. Haz commit de tus cambios: `git commit -m "feat: añadir X"`
4. Haz push: `git push origin feature/nueva-funcionalidad`
5. Abre un Pull Request

---

## 📄 Licencia

MIT License — Ver [LICENSE](./LICENSE)

---

## 📧 Contacto

**Autor**: CiGaRReTeM  
**Repositorio**: [GuiaGastronomicaHuelva](https://github.com/CiGaRReTeM/GuiaGastronomicaHuelva)

---

**Estado del proyecto**: 📝 Diseño completado — Desarrollo próximamente
