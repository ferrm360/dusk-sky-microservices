
# 🔐 AuthService

Este microservicio se encarga de gestionar la autenticación, creación y control de usuarios base para la plataforma.

Incluye dos documentos principales en MongoDB:

- `User`: información pública del usuario
- `UserSecret`: credenciales encriptadas del usuario

---

## Formato del documento `User` (MongoDB)

```json
{
  "_id": "ObjectId",
  "username": "fer_gamer",
  "email": "fer@example.com",
  "role": "player",
  "status": "active",
  "created_at": "2025-04-25T17:00:00.000Z"
}
```

### Campos

| Campo        | Tipo           | Descripción |
|--------------|----------------|-------------|
| `_id`        | `ObjectId`     | Generado automáticamente por MongoDB |
| `username`   | string (máx. 50) | Nombre de usuario, debe ser único |
| `email`      | string (máx. 250) | Correo del usuario, debe ser único |
| `role`       | enum            | Uno de: `player`, `moderator`, `admin` |
| `status`     | enum            | Uno de: `active`, `suspended`, `banned` |
| `created_at` | datetime        | Fecha automática de registro |

---

## 🔐 Formato del documento `UserSecret` (MongoDB)

```json
{
  "_id": "ObjectId",
  "user_id": "ObjectId",
  "password_hash": "$2b$10$...hash...",
  "password_salt": "$2b$10$...salt..."
}
```

### Campos

| Campo           | Tipo        | Descripción |
|------------------|-------------|-------------|
| `_id`            | `ObjectId`  | ID del documento en `UserSecret` |
| `user_id`        | `ObjectId`  | **Debe ser el mismo `_id` del documento `User` asociado** |
| `password_hash`  | string      | Contraseña encriptada con hash (ej: bcrypt) |
| `password_salt`  | string      | Salt usado en el proceso de hash |

---

### 🔗 Relación entre documentos

- El campo `user_id` de `UserSecret` es una **clave foránea lógica**
- Debe hacer referencia al `_id` de un documento en `User`
- Esto permite mantener separada la información pública de la sensible
- Se debe garantizar que no existan múltiples `UserSecret` para un mismo usuario

---

1. Se crea el documento en `User` y se genera un `_id` (ObjectId)
2. Se hashea la contraseña
3. Se crea un documento `UserSecret` usando ese `_id` como `user_id`
