
# 🎮 GameService

Este microservicio gestiona la información de los videojuegos en la plataforma.  
Incluye los detalles básicos de los juegos, detalles completos (descripción, plataformas, etc.) y las imágenes relacionadas.
Se encuentran los JSON en la carpeta Data.

---

## 📦 Formato del documento `Game` (MongoDB)

```json
{
  "_id": "066440e1-7acd-4efa-8493-62ccd898c53b",
  "steam_appid": 570,
  "name": "Dota 2"
}
```

### 📘 Descripción de campos

| Campo       | Tipo              | Descripción |
|-------------|-------------------|-------------|
| `_id`       | UUID (string)     | ID único generado por la base de datos |
| `steam_appid` | number (entero) | ID del juego en Steam, puede estar vacío si el juego se registró manualmente |
| `name`      | string            | Nombre del juego |

---

## 📦 Formato del documento `GameDetails` (MongoDB)

```json
{
  "_id": "638ec347-2197-48c5-8b2f-4bf4279b4f0d",
  "game_id": "066440e1-7acd-4efa-8493-62ccd898c53b",
  "description": "Every day, millions of players worldwide enter battle...",
  "developer": "Valve",
  "publisher": "Valve",
  "release_date": "9 Jul, 2013",
  "platforms": {
    "windows": true,
    "mac": true,
    "linux": true
  },
  "genres": [
    "8f153852-ee29-49f5-8472-80896718f774",
    "ce70cd06-8da2-407d-8008-2cdbd6ddb0f5",
    "51c46bde-5f5b-4d2e-97d5-3c05a972a819"
  ]
}
```

### 📘 Descripción de campos

| Campo        | Tipo              | Descripción |
|--------------|-------------------|-------------|
| `_id`        | UUID (string)     | ID único generado por la base de datos |
| `game_id`    | UUID (string)     | ID del juego en la colección `Game` |
| `description`| string            | Descripción completa del juego |
| `developer`  | string            | Desarrollador del juego |
| `publisher`  | string            | Publicador del juego |
| `release_date`| date (ISO)       | Fecha de lanzamiento |
| `platforms`  | object            | Plataformas en las que está disponible el juego (windows, mac, linux) |
| `genres`     | array de UUIDs    | IDs de los géneros del juego, hace referencia a la colección `Genres` |

---

## 📦 Formato del documento `Image` (MongoDB)

```json
{
  "_id": "797ff0c4-f531-4e38-9953-60713b02938a",
  "game_id": "066440e1-7acd-4efa-8493-62ccd898c53b",
  "header_url": "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/570/header.jpg?t=1745368590",
  "screenshots": [
    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/570/ss_ad8eee787704745ccdecdfde3a5cd2733704898d.1920x1080.jpg?t=1745368590",
    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/570/ss_7ab506679d42bfc0c0e40639887176494e0466d9.1920x1080.jpg?t=1745368590",
    ...
  ]
}
```

### 📘 Descripción de campos

| Campo        | Tipo              | Descripción |
|--------------|-------------------|-------------|
| `_id`        | UUID (string)     | ID único generado por la base de datos |
| `game_id`    | UUID (string)     | ID del juego en la colección `Game` |
| `header_url` | string (URL)      | URL de la imagen de cabecera del juego |
| `screenshots`| array de strings  | URLs de las capturas de pantalla del juego |

---

## 🔗 Relación entre documentos

- **`game_id`** en `GameDetails` y `Image` hace referencia al `_id` en `Game`, lo que permite acceder a los detalles completos e imágenes asociadas a un juego.
- Si `steam_appid` está vacío, significa que el juego se registró manualmente y no se tiene información de Steam.

---

## 🔐 Consideraciones

- Si `steam_appid` está vacío, el juego puede haberse registrado manualmente en lugar de importarse desde Steam.
- **Se recomienda indexar** los campos `game_id` y `steam_appid` para optimizar las consultas.
- Para ingresar nuevos juegos podemos usar el api gratuita de SteamSearch
---

