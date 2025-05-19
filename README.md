### 📦 Microservicios y Lenguajes Utilizados

| Microservicio             | Lenguaje                  | API (Puerto Host) | MongoDB/SQL (Puerto Host) | Descripción breve                                        |
| ------------------------- | ------------------------- | ----------------- | --------------------- | -------------------------------------------------------- |
| `AuthService`             | 🟢 **Python**             | `8001`            | `27017`               | Autenticación, login, registro, JWT.                     |
| `UserManagerService`      | 🟢 **Python**             | `8003`            | `27018`               | Perfil público, avatar, multimedia.                      |
| `GameService`             | 🟣 **C# (.NET)**          | `8004`            | `27019`               | Catálogo de juegos, integración con API de Steam.        |
| `UserGameTrackingService` | 🟣 **C# (.NET)**          | `8005`            | `27020`               | Seguimiento de juegos (jugando, completado, abandonado). |
| `ReviewService`           | 🟢 **Python**             | `8006`            | `27021`               | Publicación de reseñas, puntuación numérica.             |
| `FriendshipService`       | 🟢 **Python** / 🟣 **C#** | `8007`            | `27022`               | Relaciones sociales (amigos, bloqueos).                  |
| `CommentService`          | 🟣 **C# (.NET)**          | `8008`            | `27023`               | Comentarios en reseñas o perfiles.                       |
| `ModerationService`       | 🟣 **C# (.NET)**          | `8009`            | `27024`               | Reportes, sanciones y moderación.                        |
| `GameListService`         | 🟣 **C# (.NET)**          | `8010`            | `27025`               | Listas personalizadas tipo Letterboxd.                   |


| Recurso                     | Servicio Docker             | Puerto(s) Host   | Descripción                                                                    |
| --------------------------- | --------------------------- | ---------------- | ------------------------------------------------------------------------------ |
| `RabbitMQ`                  | `shared_rabbit`             | `5672`, `15672`  | Sistema de mensajería (pub/sub). `5672` para apps, `15672` para UI de gestión. |

## 🗄️ Microservicios con SQL

Los siguientes servicios utilizan bases de datos **relacionales (SQL)** por sus necesidades de integridad, relaciones entre entidades y validaciones estrictas:

| Microservicio         | Base de datos | Razón principal |
|------------------------|----------------|------------------|
| `FriendshipService`    | MySQL / MariaDB | Relación única entre pares de usuarios (A-B = B-A), fácil de validar con índices |
| `ModerationService`    | MySQL / MariaDB | Control estructurado de reportes y sanciones, con enums y relaciones 1:1 |

🧠 Ambos servicios se benefician de:
- Enums para estados (`pending`, `accepted`, `resolved`, etc.)
- Índices únicos y claves foráneas
- Mejor control de duplicados e integridad referencial
