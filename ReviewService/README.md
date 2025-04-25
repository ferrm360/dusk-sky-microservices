
# 📝 ReviewService

Este microservicio gestiona las **reseñas de videojuegos** publicadas por los usuarios.  
Incluye contenido escrito, calificación, relación con el juego y sistema de likes con prevención de duplicados.

---

## 📦 Formato del documento `Review` (MongoDB)

```json
{
  "_id": "UUID",
  "userId": "UUID",
  "gameId": "steam_appid",
  "content": "Absolutely loved it!",
  "rating": 4.5,
  "createdAt": "2025-04-06T01:00:00Z",
  "likes": 37,
  "likedBy": ["UUID", "UUID"]
}
```

---

## 📘 Descripción de campos

| Campo       | Tipo              | Descripción |
|-------------|-------------------|-------------|
| `_id`       | UUID (string)     | ID único generado por la base de datos |
| `userId`    | UUID (string)     | ID del autor de la reseña (referencia al `AuthService`) |
| `gameId`    | string            | ID del juego reseñado (ej: `steam_620`) |
| `content`   | string            | Texto de la reseña |
| `rating`    | number (0.0–5.0)  | Calificación numérica dada al juego |
| `createdAt` | datetime (ISO)    | Fecha de publicación de la reseña |
| `likes`     | number (entero)   | Número total de likes recibidos |
| `likedBy`   | array de UUIDs    | Lista de IDs de usuarios que han dado like |

---

## ❤️ Sistema de Likes

- El campo `likes` representa el total de likes recibidos
- El campo `likedBy` contiene una lista con los IDs de usuarios que han dado like
- Para evitar que un usuario repita su like:
  - Solo se puede hacer `like` si `userId` **no está en** `likedBy`
  - Solo se puede hacer `unlike` si `userId` **sí está en** `likedBy`

### Dar like

```js
await Review.updateOne(
  { _id: reviewId, likedBy: { $ne: userId } },
  {
    $push: { likedBy: userId },
    $inc: { likes: 1 }
  }
);
```

### Quitar like

```js
await Review.updateOne(
  { _id: reviewId, likedBy: userId },
  {
    $pull: { likedBy: userId },
    $inc: { likes: -1 }
  }
);
```

---

## 🔐 Consideraciones

- Se recomienda indexar `userId` y `gameId` para optimizar búsquedas
- Se puede mostrar `likes` públicamente, pero `likedBy` puede mantenerse interno
- Validar que `likes` no sea menor a 0

---

