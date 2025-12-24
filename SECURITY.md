# 🔐 Configuración Segura - API Keys y Secretos

## ⚠️ Importante: NO Commits de Secretos

Este proyecto incluye archivos de configuración sensibles que **NUNCA** deben ser comprometidos en Git:

- `appsettings.json` - Contiene API keys
- `.env` - Variables de entorno
- Cualquier archivo con credenciales

## 📋 Configuración Local

### 1. Google Places API

1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea una API Key para Google Places Text Search
3. Abre `src/GuiaGastronomica.Api/appsettings.json`
4. Reemplaza `YOUR_GOOGLE_PLACES_API_KEY_HERE` con tu clave real:

```json
"GooglePlaces": {
  "ApiKey": "AIzaSy..."
}
```

### 2. Variables de Entorno (Alternativa segura para producción)

En lugar de appsettings.json, puedes usar variables de entorno:

**Windows PowerShell:**
```powershell
$env:GooglePlaces__ApiKey = "tu_api_key_aqui"
```

**Linux/Mac:**
```bash
export GooglePlaces__ApiKey="tu_api_key_aqui"
```

**En Program.cs ya está configurado para leer de ambas fuentes.**

## ✅ Verificación de Seguridad

Antes de hacer push a GitHub:

```bash
# Ver archivos que podrían ser comprometidos
git status

# Nunca deberías ver appsettings.json aquí
# Si lo ves, está mal configurado en .gitignore
```

## 🔄 Si Accidentalmente Expusiste una Clave

1. **Revoca inmediatamente** en Google Cloud Console
2. **Crea una nueva API Key**
3. **Limpia el historio de Git:**
   ```bash
   git filter-branch --force --index-filter "git rm --cached --ignore-unmatch src/GuiaGastronomica.Api/appsettings.json" --prune-empty --tag-name-filter cat -- --all
   ```
4. **Force push** (solo si es repositorio privado):
   ```bash
   git push origin --force --all
   ```

## 📚 Recursos

- [Google Cloud Security Best Practices](https://cloud.google.com/docs/authentication/getting-started)
- [OWASP - Secrets Management](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)
- [GitHub - Handling Credentials](https://docs.github.com/en/code-security/secret-scanning)
