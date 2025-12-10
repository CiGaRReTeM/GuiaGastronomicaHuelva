# Guía Gastronómica Justa (métricas y diseño)

Fecha: 2025-12-09
Autor: CiGaRReTeM (idea original)

## Resumen

Crear una web para mostrar rankings justos de bares y restaurantes en la ciudad de Huelva. Objetivo: ofrecer un ranking transparente y no sesgado por publicidad o pagos a plataformas grandes (p. ej. TripAdvisor). Dar visibilidad y respeto a los establecimientos que realmente lo merecen.

## Objetivos principales

- Clasificar locales según señales reales de calidad (reseñas verificadas, fotos geolocalizadas, precios, consistencia del menú, tráfico real, opinión de food bloggers locales, etc.).
- Evitar el "pay-to-top" y minimizar manipulaciones mediante procesos de verificación y señales múltiples.
- Proveer una experiencia de búsqueda y descubrimiento para residentes y turistas.

## Público objetivo

- Residentes de Huelva que buscan recomendaciones fiables.
- Foodies y visitantes que quieren descubrir locales con mérito real.
- Propietarios de bares/restaurantes que buscan feedback honesto.

## MVP (versión mínima viable)

1. Lista de locales con fichas básicas (nombre, dirección, fotos, puntuación calculada).
2. Fuente de datos inicial: reseñas de usuarios (subidas manualmente), scraping básico de webs públicas y agregación de reseñas públicas.
3. Motor de scoring simple que combine: número de reseñas verificadas, sentimiento medio de reseñas, fotos geolocalizadas, menciones en blogs locales.
4. Interfaz en Blazor con búsqueda y filtros (zona, tipo de cocina, rango de precio).
5. Panel administrativo para validar reseñas y gestionar denuncias.

## Fuentes de datos (ideas)

- APIs públicas (si existen) de plataformas de reseñas.
- Scraping de sitios web (respetando TOS y robots.txt).
- Redes sociales: Instagram (hashtags, geotags), X/Twitter, Facebook (public posts), Google Maps (cuando sea posible mediante API).
- Fotos geolocalizadas (EXIF) compartidas públicamente.
- Menús y precios desde webs o PDFs públicos.
- Reseñas y artículos de food bloggers locales.

Nota: respetar términos de servicio, políticas de uso y privacidad; preferir APIs oficiales y fuentes con permiso.

## IA y RAG (propuesta técnica)

- Lenguaje y stack: .NET (C#) para backend y Blazor para frontend (solicitado por el autor).
- Biblioteca de integración LLM: Semantic Kernel (.NET) para orquestar prompts, embeddings y pipeline RAG.
- Embeddings + vector store para RAG: opciones recomendadas para empezar
  - RAG prototipo: vector store en memoria o archivo local (para pruebas).
  - Producción: Qdrant (autohospedado con Docker) o PostgreSQL + pgvector.
- Modelos LLM: priorizar LLMs gratuitos/autohospedados cuando sea posible (ej.: modelos en Hugging Face, local con llama.cpp/ggml, Ollama si el usuario lo instala). Mantener opción de usar APIs comerciales si el usuario lo decide.

## Arquitectura propuesta (alto nivel)

- Frontend: Blazor WebAssembly (o Blazor Server según preferencia) para UI interactiva.
- Backend: ASP.NET Core Web API
  - Endpoints REST para búsqueda, ficha de local, envío de reseñas, administración.
  - Servicio de ingestión: colecciona datos de fuentes configuradas y normaliza entradas.
  - Servicio de scoring: calcula puntuaciones y justificaciones (explicables) basadas en señales.
  - Servicio RAG: indexa textos y documentos, gestiona embeddings y responde preguntas mediante Semantic Kernel.
- Almacenamiento:
  - Base relacional: PostgreSQL (o SQLite para prototipo).
  - Vector DB: Qdrant / pgvector / in-memory para pruebas.
  - Almacenamiento de archivos (fotos): local o S3 compatible.

## Privacidad, legal y moderación

- Verificar que el scraping y la recolección de datos cumplen ToS y legislación local (GDPR si aplica).
- Implementar moderación y verificación de reseñas (para evitar spam y pagos fraudulentos).
- Proveer mecanismo de reclamaciones para locales.

## Métricas de éxito (KPI)

- Nº de locales indexados y validados.
- Nº de reseñas verificadas vs no verificadas.
- Tasa de aceptación de reseñas (moderación).
- Tráfico y uso (visitas únicas, búsquedas realizadas).

## Próximos pasos propuestos

1. Acordar MVP exacto y prioridades.
2. Elegir variantes concretas del stack (Blazor Server vs WASM; Qdrant vs pgvector; modelo LLM preferido).
3. Crear scaffold mínimo: backend ASP.NET Core mínimamente funcional, Blazor frontend con lista estática y CI básico.
4. Implementar pipeline de ingestión de datos para 2-3 fuentes iniciales.
5. Prototipar RAG con datos locales y Semantic Kernel.

---

Si estás de acuerdo, puedo ahora:
- Proponer una estructura de carpetas y comandos para crear el scaffold (proyecto .NET + Blazor), o
- Crear el scaffold mínimo directamente aquí (proyecto .NET, archivos iniciales y README actualizado).

Indícame cómo quieres avanzar.
