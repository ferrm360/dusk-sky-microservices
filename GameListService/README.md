
# 🎮 GameListService

Este microservicio gestiona las **listas de juegos** que los usuarios pueden crear.  
Permite a los usuarios organizar sus juegos favoritos, recomendados, jugados, etc., en listas personalizadas.
---

## 📦 Formato del documento `GameList` (MongoDB)

```json
{
  "_id": "66be8c6f-7454-4d10-9279-bb0146b4b09b",
  "user_id": "user_uuid",
  "name": "My Favorite RPGs",
  "description": "A collection of my favorite RPG games",
  "is_public": true,
  "created_at": "2025-04-06T01:00:00Z"
}
```

### 📘 Descripción de campos

| Campo         | Tipo            | Descripción |
|---------------|-----------------|-------------|
| `_id`         | UUID (string)   | ID único generado por la base de datos |
| `user_id`     | UUID (string)   | ID del usuario que crea la lista (referencia al `AuthService`) |
| `name`        | string          | Nombre de la lista de juegos |
| `description` | string          | Descripción de la lista |
| `is_public`   | boolean         | Indica si la lista es pública o privada |
| `created_at`  | datetime (ISO)  | Fecha de creación de la lista |

---

## 📦 Formato del documento `GameListItem` (MongoDB)

```json
{
  "_id": "f6a1f8c6-282b-4c8d-9119-59b763c01e3e",
  "list_id": "66be8c6f-7454-4d10-9279-bb0146b4b09b",
  "game_id": "game_uuid",
  "comment": "Great game!"
}
```

### 📘 Descripción de campos

| Campo     | Tipo            | Descripción |
|-----------|-----------------|-------------|
| `_id`     | UUID (string)   | ID único generado por la base de datos |
| `list_id` | UUID (string)   | ID de la lista en la colección `GameList` |
| `game_id` | UUID (string)   | ID del juego en la colección `GameService` |
| `comment` | string          | Comentario adicional del usuario sobre el juego |

---

## 🔗 Relación entre documentos

- **`GameList`**: Contiene las listas que los usuarios pueden crear, y tiene una relación con los documentos `GameListItem` a través de `list_id`.
- **`GameListItem`**: Cada juego en la lista tiene una relación con `GameList` a través de `list_id` y con `GameService` a través de `game_id`.

---

## 🔄 ¿Cómo gestionar las relaciones?

1. **Crear una lista**: Un usuario puede crear una lista de juegos. Esto genera un documento en `GameList`.
2. **Agregar un juego a la lista**: Para cada juego agregado a la lista, se crea un documento en `GameListItem` que contiene `list_id` (referencia a la lista) y `game_id` (referencia al juego).
3. **Consultar las listas**: Para obtener todos los juegos de una lista, se hace una consulta en `GameListItem` con el `list_id` de la lista. Luego, para obtener los detalles de los juegos, se consulta la base de datos de `GameService` usando el `game_id`.


## 🔐 Consideraciones

- **Consultas eficientes**: Se recomienda indexar los campos `user_id` en `GameList` y `game_id` en `GameListItem` para mejorar el rendimiento de las consultas.
- **Flexibilidad**: MongoDB permite **agregar o quitar campos** fácilmente. Si se necesita agregar más detalles a los juegos dentro de las listas (como `rating` o `status`), esto se puede hacer fácilmente sin tener que cambiar la estructura de la base de datos.
- **Escalabilidad**: Este enfoque con referencias permite escalar de manera eficiente, especialmente si las listas de juegos tienen muchos elementos.
