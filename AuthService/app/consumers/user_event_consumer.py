import asyncio
import json
import aio_pika
from motor.motor_asyncio import AsyncIOMotorDatabase

from app.controllers import auth_controller
from app.config.settings import settings


RABBITMQ_URL = settings.RABBITMQ_URL
# Nombre del exchange que usará userManagerService para publicar y este consumidor para escuchar
EXCHANGE_NAME = 'user_events_exchange'
# Nombre de la cola específica para que authService consuma los mensajes
QUEUE_NAME = 'auth_service_user_updates_queue'
# Lista de "routing keys" o "temas" a los que este consumidor se suscribirá.
# Deben coincidir con las routing keys que userManagerService use al publicar.
ROUTING_KEYS_TO_BIND = [
    "user.username.updated", # Evento para cuando se actualiza un nombre de usuario
    "user.email.updated",    # Evento para cuando se actualiza un email
    "user.deleted",           # Evento para cuando se elimina un usuario
    "user.changePassword"
    # Puedes agregar más routing keys aquí si necesitas escuchar otros eventos
]

# --- Funciones para procesar cada tipo de evento ---
# Estas funciones toman el 'payload' del mensaje de RabbitMQ y la instancia de la base de datos.
# Luego, llaman a las funciones correspondientes en tu 'auth_controller.py'.

async def process_username_updated(payload: dict, db: AsyncIOMotorDatabase):
    """
    Procesa un evento de actualización de nombre de usuario.
    Llama a la función en auth_controller para actualizar la base de datos de authService.
    """
    try:
        user_id = payload.get("user_id")
        new_username = payload.get("new_username")

        if not user_id or new_username is None: # new_username puede ser una cadena vacía, ajusta la validación si es necesario
            print(f" [CONSUMER ERROR] Mensaje USERNAME_UPDATED inválido: falta user_id o new_username. Payload: {payload}")
            return # No se puede procesar sin la información necesaria

        print(f" [CONSUMER INFO] Procesando evento USERNAME_UPDATED para user_id: {user_id} a '{new_username}'")
        # Llama a la función de tu controlador que se encarga de la lógica de negocio y la BD
        await auth_controller.update_username(user_id=user_id, new_username=new_username, db=db)
        print(f" [CONSUMER SUCCESS] Username para {user_id} actualizado en la BD de authService.")

    except ValueError as e: # Captura errores específicos que pueda lanzar tu auth_controller (ej. Usuario no encontrado)
        print(f" [CONSUMER ERROR] Error de valor al procesar USERNAME_UPDATED para user_id '{user_id}': {e}")
    except Exception as e: # Captura cualquier otro error inesperado
        print(f" [CONSUMER ERROR] Error inesperado al procesar USERNAME_UPDATED para user_id '{user_id}': {e}")
        # Considera aquí una lógica de reintento o enviar a una cola de "dead-letter" si es un error persistente.

async def process_email_updated(payload: dict, db: AsyncIOMotorDatabase):
    try:
        user_id = payload.get("user_id")
        new_email = payload.get("new_email")

        if not user_id or not new_email:
            print(f" [CONSUMER ERROR] Mensaje USER_EMAIL_UPDATED inválido: falta user_id o new_email. Payload: {payload}")
            return

        print(f" [CONSUMER INFO] Procesando evento USER_EMAIL_UPDATED para user_id: {user_id} a '{new_email}'")
        await auth_controller.update_email(user_id=user_id, new_email=new_email, db=db)
        print(f" [CONSUMER SUCCESS] Email para {user_id} actualizado en la BD de authService.")

    except ValueError as e:
        print(f" [CONSUMER ERROR] Error de valor al procesar USER_EMAIL_UPDATED para user_id '{user_id}': {e}")
    except Exception as e:
        print(f" [CONSUMER ERROR] Error inesperado al procesar USER_EMAIL_UPDATED para user_id '{user_id}': {e}")

async def process_user_deleted(payload: dict, db: AsyncIOMotorDatabase):
    try:
        user_id = payload.get("user_id")

        if not user_id:
            print(f" [CONSUMER ERROR] Mensaje USER_DELETED inválido: falta user_id. Payload: {payload}")
            return

        print(f" [CONSUMER INFO] Procesando evento USER_DELETED para user_id: {user_id}")
        await auth_controller.delete_user(user_id=user_id, db=db) # Asume que tienes esta función en auth_controller
        print(f" [CONSUMER SUCCESS] Usuario {user_id} procesado para eliminación en la BD de authService.")

    except ValueError as e:
        print(f" [CONSUMER ERROR] Error de valor al procesar USER_DELETED para user_id '{user_id}': {e}")
    except Exception as e:
        print(f" [CONSUMER ERROR] Error inesperado al procesar USER_DELETED para user_id '{user_id}': {e}")


# --- Función principal del Consumidor de RabbitMQ ---
async def consume_user_events(db: AsyncIOMotorDatabase):
    """
    Función principal que se conecta a RabbitMQ, declara la topología (exchange, cola, bindings)
    y comienza a escuchar mensajes. Esta función se ejecuta como una tarea de fondo en FastAPI.
    """
    connection = None # Para manejar la conexión en el bucle de reintento
    print(f" [CONSUMER INFO] Intentando conectar a RabbitMQ en: {RABBITMQ_URL}")

    # Bucle infinito para reintentar la conexión si se pierde
    while True:
        try:
            # Conexión robusta que intenta reconectar automáticamente si hay problemas temporales
            connection = await aio_pika.connect_robust(RABBITMQ_URL)
            print(f" [CONSUMER SUCCESS] Conectado a RabbitMQ en {RABBITMQ_URL}!")

            async with connection:
                # Crear un canal
                channel = await connection.channel()

                # Configurar Quality of Service (QoS): no tomar más de X mensajes a la vez
                # hasta que los anteriores hayan sido procesados (acknowledged).
                # Esto evita que el consumidor se sature si hay muchos mensajes.
                await channel.set_qos(prefetch_count=10)

                # Declarar el Exchange (debe ser del mismo tipo y nombre que el productor)
                # 'durable=True' significa que el exchange sobrevivirá a reinicios del broker RabbitMQ.
                exchange = await channel.declare_exchange(
                    EXCHANGE_NAME, aio_pika.ExchangeType.DIRECT, durable=True
                )

                # Declarar la Cola donde se almacenarán los mensajes para este consumidor
                # 'durable=True' significa que la cola (y sus mensajes, si son persistentes)
                # sobrevivirá a reinicios del broker.
                queue = await channel.declare_queue(QUEUE_NAME, durable=True)

                # Vincular (bind) la cola al exchange con las routing keys especificadas.
                # La cola recibirá mensajes del exchange que tengan estas routing keys.
                for r_key in ROUTING_KEYS_TO_BIND:
                    await queue.bind(exchange, routing_key=r_key)
                    print(f" [CONSUMER INFO] Cola '{QUEUE_NAME}' vinculada a exchange '{EXCHANGE_NAME}' con routing key '{r_key}'")

                print(f" [CONSUMER INFO] Esperando mensajes en la cola '{QUEUE_NAME}'. Para salir, detén la aplicación.")

                # Empezar a consumir mensajes de la cola de forma asíncrona
                async with queue.iterator() as queue_iter:
                    async for message in queue_iter:
                        # message.process() es un context manager que automáticamente hace acknowledge (ack)
                        # del mensaje si el bloque de código dentro del 'with' se completa sin errores.
                        # Si hay un error dentro del bloque, y no se captura, el mensaje se reencolará (nack con requeue=True por defecto).
                        # `ignore_processed=True` puede ser útil si manejas el ack/nack explícitamente,
                        # pero el comportamiento por defecto de message.process() suele ser bueno.
                        async with message.process(ignore_processed=False): # False (default) para que aio-pika maneje el ack/nack básico
                            try:
                                body_str = message.body.decode()
                                print(f"\n [CONSUMER INFO] Mensaje recibido! Routing Key: '{message.routing_key}', Body: '{body_str}'")
                                
                                data = json.loads(body_str)
                                event_type = data.get("event_type") # Ej: "USERNAME_UPDATED"
                                payload = data.get("payload", {})   # El contenido del mensaje

                                # Aquí decides qué función de procesamiento llamar basado en el tipo de evento o la routing key
                                if message.routing_key == "user.username.updated" and event_type == "USERNAME_UPDATED":
                                    await process_username_updated(payload, db)
                                elif message.routing_key == "user.email.updated" and event_type == "USER_EMAIL_UPDATED":
                                    await process_email_updated(payload, db)
                                elif message.routing_key == "user.deleted" and event_type == "USER_DELETED":
                                    await process_user_deleted(payload, db)
                                else:
                                    print(f" [CONSUMER WARNING] Evento o routing key no reconocido/manejado: type='{event_type}', key='{message.routing_key}'. Mensaje ignorado (ack).")
                                    # Si el mensaje es desconocido pero no quieres que se reencole, se ackea igual al salir del with.

                            except json.JSONDecodeError:
                                print(f" [CONSUMER ERROR] Fallo al decodificar cuerpo del mensaje JSON: '{message.body.decode()[:100]}...' El mensaje será rechazado (nack) y no reencolado.")
                                # Si `message.process()` está activo y ocurre este error, se nackeará sin reencolar.
                                # Si necesitaras control explícito: await message.reject(requeue=False)
                            except Exception as e:
                                print(f" [CONSUMER ERROR] Error crítico procesando mensaje (routing_key: {message.routing_key}): {e}. El mensaje podría ser reencolado.")
                                # Si `message.process()` está activo, este error causará un nack y reencolado,
                                # lo cual puede ser bueno para errores transitorios, pero peligroso si es un error persistente.
                                # Considera una estrategia de "dead-letter queue" para errores persistentes.
                                raise # Re-lanzar la excepción para que message.process() la maneje (nack y requeue)
        
        except aio_pika.exceptions.AMQPConnectionError as e:
            print(f" [CONSUMER ERROR] Error de conexión con RabbitMQ (AMQPConnectionError): {e}. Reintentando en 5 segundos...")
        except ConnectionRefusedError as e:
            print(f" [CONSUMER ERROR] Conexión con RabbitMQ rechazada (ConnectionRefusedError): {e}. ¿Está RabbitMQ corriendo y accesible? Reintentando en 5 segundos...")
        except Exception as e: # Captura cualquier otro error que impida la conexión o configuración inicial
            print(f" [CONSUMER ERROR] Error inesperado en la configuración del consumidor de RabbitMQ: {e}. Reintentando en 5 segundos...")
        
        finally:
            if connection and not connection.is_closed:
                try:
                    print(" [CONSUMER INFO] Cerrando conexión actual de RabbitMQ antes de reintentar o salir...")
                    await connection.close()
                except Exception as e_close:
                    print(f" [CONSUMER WARNING] Error al cerrar la conexión de RabbitMQ: {e_close}")
            # Esperar 5 segundos antes de reintentar la conexión en caso de fallo
            print(" [CONSUMER INFO] Esperando 5 segundos antes del próximo intento de conexión a RabbitMQ...")
            await asyncio.sleep(5)