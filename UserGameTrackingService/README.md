# 🕹️ UserGameTrackingService

Este microservicio gestiona el **seguimiento de juegos** que hacen los usuarios: qué están jugando, han jugado, planean jugar o abandonaron, junto con su calificación y “like”.  

---

## 📦 Formato del documento (MongoDB / JSON)

```json
{
  "_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "user_id": "65aa3e7af1bd84dca3a23841",
  "game_id": "066440e1-7acd-4efa-8493-62ccd898c53b",
  "status": "played",
  "rating": 4.5,
  "liked": true,
  "updated_at": "2025-04-25T18:00:00Z"
}
```

| Campo         | Tipo               | Descripción                                            |
|---------------|--------------------|--------------------------------------------------------|
| `_id`         | UUID (string)      | ID único del seguimiento (generado por MongoDB)        |
| `user_id`     | UUID (string)      | Referencia al usuario en **AuthService** (validado vía API) |
| `game_id`     | UUID (string)      | Referencia al juego en **GameService** (validado vía API)  |
| `status`      | string             | Uno de: `played`, `playing`, `backlog`, `abandoned`    |
| `rating`      | number (0.0–5.0)   | Calificación del juego                                 |
| `liked`       | boolean            | Si el usuario marcó “me gusta”                         |
| `updated_at`  | ISO datetime (UTC) | Fecha de última actualización                          |

---

## 🔗 Referencias entre servicios

- **`user_id`** y **`game_id`** son **referencias lógicas** a otros microservicios.
- **No hay claves foráneas** a nivel de base de datos entre colecciones distintas.
- Antes de insertar o actualizar un documento, **el código cliente** debe:
  1. Llamar a `GET /api/users/{user_id}` en **AuthService**  
  2. Llamar a `GET /api/games/{game_id}` en **GameService**  
  3. Confirmar que ambos existen y tienen estado válido.

---
