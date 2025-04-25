
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
| `gameId`    | string            | ID del juego reseñado. Hace referencia al servicio `GameService` donde se almacena la información detallada del juego |
| `content`   | string            | Texto de la reseña |
| `rating`    | number (0.0–5.0)  | Calificación numérica dada al juego |
| `createdAt` | datetime (ISO)    | Fecha de publicación de la reseña |
| `likes`     | number (entero)   | Número total de likes recibidos |
| `likedBy`   | array de UUIDs    | Lista de IDs de usuarios que han dado like |

---

## ❤️ Sistema de Likes

- El campo `likes` representa el total de likes recibidos.
- El campo `likedBy` contiene una lista con los IDs de usuarios que han dado like.
- Para evitar que un usuario repita su like:
  - Solo se puede hacer `like` si `userId` **no está en** `likedBy`.
  - Solo se puede hacer `unlike` si `userId` **sí está en** `likedBy`.

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

- Se recomienda indexar `userId` y `gameId` para optimizar búsquedas.
- Se puede mostrar `likes` públicamente, pero `likedBy` debe mantenerse interno para evitar exponer IDs de usuarios.
- Es importante validar que el campo `likes` no sea menor a 0.

---


## 🔗 Relación con otros servicios

- **`userId`**: Este campo hace referencia al **usuario que escribió la reseña**. El valor de `userId` corresponde a un documento en el **`AuthService`**.  

- **`gameId`**: Este campo hace referencia al **juego sobre el cual se ha escrito la reseña**. El valor de `gameId` corresponde a un documento en el **`GameService`**.  

### ¿Cómo se gestionan estas relaciones?

1. Cuando un usuario escribe una reseña, solo se guarda su `userId` (referencia a `AuthService`) y el `gameId` (referencia a `GameService`).
2. Cuando mostramos una reseña, usamos esas referencias para hacer consultas a **`AuthService`** y **`GameService`** para obtener los detalles de usuario y juego.


---
