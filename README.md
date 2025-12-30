# Guía Gastronómica Justa

**Rankings justos y transparentes de bares y restaurantes en Huelva**

---

## 📋 Descripción

Plataforma web para mostrar rankings de locales gastronómicos en Huelva basados en señales reales de calidad, sin sesgos publicitarios ni "pay-to-top". Combina reseñas verificadas, análisis de sentimiento con IA, geolocalización, y feedback comunitario en tiempo real mediante chatbot conversacional.

**Principio fundamental**: 100% componentes gratuitos y open-source para desarrollo. Único coste: hosting en producción (~5-10 €/mes).

---

## 🎯 Características principales

- ✅ **Ranking justo**: algoritmo transparente basado en señales múltiples (no solo reseñas).
- ✅ **Mapa interactivo**: rankings por zonas/barrios con Leaflet.js + OpenStreetMap.
- 🤖 **Chatbot con IA**: interacción conversacional para captar feedback, verificar datos y actualizar rankings.
- ✅ **Búsqueda y filtros**: por zona, tipo de cocina, rango de precio.
- 🟡 **Panel admin**: moderación de reseñas, gestión de denuncias (diseñado, pendiente UI).
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

- **[STATUS.md](./STATUS.md)** — Estado actual del proyecto, features completadas y pendientes ⭐ *Comienza aquí*
- **[GUIDE.md](./GUIDE.md)** — Documento de diseño completo (MVP, roadmap, arquitectura, chatbot IA)
- **[BLAZOR-MUDBLAZOR-GUIDE.md](./BLAZOR-MUDBLAZOR-GUIDE.md)** — Guía de desarrollo para frontend
- **[SETUP.md](./SETUP.md)** — Instrucciones de instalación y setup
- **[README-SCRIPTS.md](./README-SCRIPTS.md)** — Guía de scripts de ejecución

---

## 🚀 Próximos pasos

1. ✅ **Diseño y arquitectura definidos** — Ver [GUIDE.md](./GUIDE.md)
2. ✅ **Scaffold .NET y frontend** — API + Blazor WASM + modelos compartidos
3. ✅ **Mapa interactivo funcional** — Leaflet.js + OpenStreetMap
4. 🔨 **Refinamientos del mapa y página de detalle de venue** — En progreso
5. 🟡 **Chatbot IA mejorado** — Integrar Semantic Kernel + Ollama
6. 🟡 **Panel administrativo** — Moderación y gestión
7. 🟡 **Background jobs y caché** — Hangfire + Redis
8. 🟡 **Despliegue en VPS** — Docker Compose + dominio + SSL

Para más detalles: **[STATUS.md](./STATUS.md)**

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

**Estado del proyecto**: ✅ **MVP funcional** — Características core completadas, mapa interactivo operativo, pendiente refinamientos y chatbot IA avanzado. Ver [STATUS.md](./STATUS.md)
