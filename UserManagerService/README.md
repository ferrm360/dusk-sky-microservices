# 👤 UserManagerService

Este microservicio gestiona la **información de perfil público** de los usuarios de la plataforma, similar al apartado "About me" que verías en sitios como osu! o Letterboxd. Permite almacenar avatar, biografía y contenido multimedia adicional.

---

## 📦 Formato del documento `UserProfile` (MongoDB / JSON)

```json
{
  "_id": "UUID",
  "userId": "UUID",
  "avatarUrl": "https://example.com/avatar.jpg",
  "bannerUrl": "https://example.com/banner.jpg",
  "bio": "I love indie games.",
  "aboutSection": "I post reviews and screenshots.",
  "media": [
    { "type": "video", "url": "https://example.com/video.mp4" },
    { "type": "image", "url": "https://example.com/screenshot.png" }
  ],
  "favoriteGenres": ["rpg", "fps"]
}
```

### 📘 Descripción de campos

| Campo            | Tipo                | Descripción |
|------------------|---------------------|-------------|
| `_id`            | ObjectId / UUID     | ID único generado por MongoDB |
| `userId`         | UUID (string)       | Referencia al usuario en **AuthService** |
| `avatarUrl`      | string (URL)        | Enlace a la imagen de perfil del usuario |
| `bio`            | string              | Texto corto de presentación personal |
|`bannerUrl`	     |string (URL)	       | Enlace a la imagen de banner (cabecera) del perfil
| `aboutSection`   | string              | Descripción más extensa: intereses, experiencia, etc. |
| `media`          | array de objetos    | Contenido multimedia asociado al perfil |
| `media[].type`   | enum<string>        | Tipo de contenido: `video`, `image`, `link` |
| `media[].url`    | string (URL)        | URL al recurso multimedia |
| `favoriteGenres` | array de strings    | Géneros de juegos favoritos del usuario |

---

## 🔗 Referencias y validaciones

- **`userId`** debe existir en **AuthService** (validación vía API).
- Los URLs en `avatarUrl` y `media[].url` deben ser accesibles públicamente.
- `media` y `favoriteGenres` son opcionales: pueden omitirse o estar vacíos.

---

## ✅ Ventajas de usar JSON / MongoDB

1. **Flexibilidad de esquema:** Permite extender el perfil con nuevos campos (por ejemplo, `bannerUrl`, `theme`) sin migraciones.
2. **Escalabilidad:** Fácil de escalar horizontalmente para millones de perfiles.
3. **Desacoplamiento:** Cada microservicio (AuthService, UserManagerService) gestiona su propio esquema sin interferir.

---

## 📖 Flujo de uso

1. **Obtener perfil**  
   ```http
   GET /api/user/profile/{userId}
   ```
2. **Crear o actualizar perfil**  
   ```http
   PUT /api/user/profile/{userId}
   Content-Type: application/json
   ```
   ```json
   {
     "avatarUrl": "...",
     "bio": "...",
     "aboutSection": "...",
     "media": [...],
     "favoriteGenres": [...]
   }
   ```
