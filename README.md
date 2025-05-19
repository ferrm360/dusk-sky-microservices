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
- 

---

# 🌐 Uso de NGINX como Gateway Reverso en el Proyecto

Este proyecto utiliza **NGINX** como gateway reverso para centralizar el acceso a los microservicios. Esta guía explica cómo preparar y usar NGINX en entorno local, incluyendo HTTPS con certificados de desarrollo.

---

## 📦 ¿Qué hace el `nginx_gateway`?

- Redirige rutas como `/auth`, `/comments`, `/games`, etc., al contenedor correcto.
- Permite acceso por `http://localhost/...` y `https://localhost/...`.
- Usa certificados locales para simular HTTPS en desarrollo.

---

## ✅ Requisitos

### 1. Tener instalado [mkcert](https://github.com/FiloSottile/mkcert)

Para generar certificados válidos localmente:


# 🧩 Cómo agregar un nuevo microservicio al ecosistema

Este proyecto usa Docker Compose, NGINX como gateway y una red compartida para conectar múltiples microservicios. Aquí te explico cómo agregar uno nuevo correctamente.

## 🛠️ Paso 1 Agregar el servicio en `docker-compose.yml`

Agrega el nuevo servicio al archivo `docker-compose.yml`. Por ejemplo, para un servicio llamado `AuthService`:

```yaml
authservice:
    build:
        context: ./AuthService
        dockerfile: Dockerfile
    environment:
        - VAR1=valor
        - VAR2=valor
    ports:
        - "8001:80" # Puerto externo:interno
    depends_on:
        - mongodb
    networks:
        - dusk_sky_shared_network
```

--- 🧩 Cómo agregar un nuevo microservicio al ecosistema

Este proyecto usa Docker Compose, NGINX como gateway y una red compartida para conectar múltiples microservicios. Aquí te explico cómo agregar uno nuevo correctamente.

---

## ✅ Requisitos previos

- El microservicio debe estar contenido en su propia carpeta (por ejemplo: `./AuthService/`)
- Debe estar configurado para escuchar en el puerto interno `80`
- Debe conectarse a la red `dusk_sky_shared_network`
- Debe tener una ruta prefijo única (por ejemplo: `/auth`, `/games`, etc.)

---

## 🛠️ Paso 2: Configurar NGINX para enrutar al nuevo servicio

Edita el archivo `nginx.conf` para agregar una nueva entrada que enrute las solicitudes al nuevo servicio. Por ejemplo:

```nginx
location /auth/ {
        proxy_pass http://authservice:80/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
}
```

NOTA: Algunos ya están comentados, pero puedes descomentarlos si es necesario.

---

## 🛠️ Paso 3: Configurar las variables de entorno

Agrega las variables necesarias para el nuevo servicio en el archivo `.env`. Por ejemplo:

```env
AUTHSERVICE_SECRET=tu_secreto
AUTHSERVICE_DB_URI=mongodb://mongodb:27017/auth 
```

##  Cómo agregar una nueva base de datos compartiendo la instancia de MongoDB
Este proyecto utiliza una única instancia de MongoDB para múltiples bases de datos, cada una asociada a un microservicio. Aquí te explico cómo agregar una nueva base de datos para un microservicio existente o uno nuevo.

## ✅ Requisitos previos
El microservicio debe estar configurado para conectarse a la instancia de MongoDB compartida.
Debes definir un usuario y una base de datos específicos para el nuevo microservicio.
Asegúrate de que el archivo createDataBaseUser.js incluya la lógica para crear el usuario y la base de datos.

## 🛠️ Paso 1: Agregar las variables de entorno

Define las variables necesarias para el nuevo usuario y base de datos en el archivo .env. Por ejemplo, para un microservicio llamado

```
REVIEWSERVICE_MONGO_DATABASE=ReviewService
REVIEWSERVICE_MONGO_USER=reviewUser
REVIEWSERVICE_MONGO_PASSWORD=reviewUserPassword
```

## 🛠️ Paso 2: Actualizar el archivo createDataBaseUser.js
Edita el archivo scripts/createDataBaseUser.js para incluir la creación del usuario y la base de datos para el nuevo microservicio. Por ejemplo

```javascript
// Usuario para ReviewService
db = db.getSiblingDB('${REVIEWSERVICE_MONGO_DATABASE}');
db.createUser({
  user: '${REVIEWSERVICE_MONGO_USER}',
  pwd: '${REVIEWSERVICE_MONGO_PASSWORD}',
  roles: [{ role: 'readWrite', db: '${REVIEWSERVICE_MONGO_DATABASE}' }]
});
```

## 🛠️ Paso 3: Verificar la configuración en docker-compose.yml
Asegúrate de que el microservicio tenga las variables de entorno necesarias en el archivo docker-compose.yml. Por ejemplo:

Asegúrate de que el microservicio tenga las variables de entorno necesarias en el archivo docker-compose.yml. Por ejemplo:

```yaml
  reviewservice:
    build:
      context: ./ReviewService
      dockerfile: Dockerfile
    environment:
      - REVIEWSERVICE_MONGO_DATABASE=${REVIEWSERVICE_MONGO_DATABASE}
      - REVIEWSERVICE_MONGO_USER=${REVIEWSERVICE_MONGO_USER}
      - REVIEWSERVICE_MONGO_PASSWORD=${REVIEWSERVICE_MONGO_PASSWORD}
      - MONGO_HOST=mongodb
    ports:
      - "8006:80"
    depends_on:
      - mongodb
    networks:
      - dusk_sky_shared_network
```


## 🛠️ Levantar los contenedores

Ejecuta el siguiente comando para construir y levantar todos los servicios:

```bash
docker-compose up --build
```

## Rutas para probar
Las rutas quedan asi dependieno del microservicio que hayas agregado:

```bash
http://localhost/comments
http://localhost/auth
http://localhost/games
```