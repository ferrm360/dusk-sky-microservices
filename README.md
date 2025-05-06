### 📦 Microservicios y Lenguajes Utilizados

| Microservicio             | Lenguaje        | Descripción breve |
|---------------------------|-----------------|--------------------|
| `AuthService`             | 🟢 **Phyton**   | Autenticación, login, registro, JWT. Express + Passport.js. |
| `UserManagerService`      | 🟢 **Phyton**   | Perfil público, avatar, multimedia. Express + Multer. |
| `GameService`             | 🟣 **C# (.NET)** | Catálogo de juegos, integración con API de Steam. |
| `UserGameTrackingService` | 🟣 **C# (.NET)** | Seguimiento personal de juegos: jugando, completado, abandonado. |
| `ReviewService`           | 🟢 **Phyton**      | Publicación de reseñas, puntuación numérica. |
| `FriendshipService`       | 🟢 **phyton** / 🟣 **C#** | Relaciones sociales (amigos, bloqueos). |
| `CommentService`          | 🟣 **C# (.NET)** | Comentarios en reseñas o perfiles. CRUD sencillo. |
| `ModerationService`       | 🟣 **C#** | Reportes, sanciones y moderación. |
| `GameListService`         | 🟣 **C# (.NET)** | Listas personalizadas de videojuegos (tipo Letterboxd). |

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
