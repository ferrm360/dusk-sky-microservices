
# 💬 CommentService

Este microservicio administra los **comentarios** que los usuarios pueden dejar en reseñas de videojuegos.  
Incluye control de visibilidad, autoría y fecha.

---

## 📦 Formato del documento `Comment` (MongoDB)

```json
{
  "_id": "UUID",
  "reviewId": "UUID",
  "authorId": "UUID",
  "text": "Totally agree!",
  "date": "2025-04-06T01:30:00Z",
  "status": "visible"
}
```

---

## 📘 Descripción de campos

| Campo       | Tipo           | Descripción |
|-------------|----------------|-------------|
| `_id`       | ObjectId       | Generado automáticamente por MongoDB al crear el documento |
| `reviewId`  | UUID (string)  | Referencia a la reseña que está comentando (de `ReviewService`) |
| `authorId`  | UUID (string)  | ID del usuario que hace el comentario, proveniente del `AuthService` |
| `text`      | string         | Contenido del comentario |
| `date`      | datetime (ISO) | Fecha de publicación del comentario |
| `status`    | enum           | Estado del comentario: `visible`, `hidden`, `deleted`, etc. |

---

## 🔗 Relaciones con otros servicios

- `authorId` debe coincidir con el `_id` del documento `User` en `AuthService`.
- `reviewId` debe coincidir con el `_id` del documento `Review` en `ReviewService`.

---

## 🔐 Consideraciones

- Se recomienda indexar `reviewId` y `authorId` para consultas eficientes
- El campo `status` se puede usar para ocultar o eliminar comentarios sin borrarlos de la base de datos

---
