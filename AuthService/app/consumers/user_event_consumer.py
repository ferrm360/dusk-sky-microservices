import asyncio
import json
import aio_pika
from motor.motor_asyncio import AsyncIOMotorDatabase

from app.controllers import auth_controller
from app.config.settings import settings


RABBITMQ_URL = settings.RABBITMQ_URL
EXCHANGE_NAME = 'user_events_exchange'
QUEUE_NAME = 'auth_service_user_updates_queue'
ROUTING_KEYS_TO_BIND = [
    "user.username.updated", 
    "user.email.updated",   
    "user.deleted",           
    "user.changePassword"
]

async def process_username_updated(payload: dict, db: AsyncIOMotorDatabase):
    try:
        user_id = payload.get("user_id")
        new_username = payload.get("new_username")

        if not user_id or new_username is None:
            print(f" [CONSUMER ERROR] Mensaje USERNAME_UPDATED inválido: falta user_id o new_username. Payload: {payload}")
            return 

        print(f" [CONSUMER INFO] Procesando evento USERNAME_UPDATED para user_id: {user_id} a '{new_username}'")
        await auth_controller.update_username(user_id=user_id, new_username=new_username, db=db)
        print(f" [CONSUMER SUCCESS] Username para {user_id} actualizado en la BD de authService.")

    except ValueError as e:
        print(f" [CONSUMER ERROR] Error de valor al procesar USERNAME_UPDATED para user_id '{user_id}': {e}")
    except Exception as e:
        print(f" [CONSUMER ERROR] Error inesperado al procesar USERNAME_UPDATED para user_id '{user_id}': {e}")
       
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
        await auth_controller.delete_user(user_id=user_id, db=db) 
        print(f" [CONSUMER SUCCESS] Usuario {user_id} procesado para eliminación en la BD de authService.")

    except ValueError as e:
        print(f" [CONSUMER ERROR] Error de valor al procesar USER_DELETED para user_id '{user_id}': {e}")
    except Exception as e:
        print(f" [CONSUMER ERROR] Error inesperado al procesar USER_DELETED para user_id '{user_id}': {e}")


async def consume_user_events(db: AsyncIOMotorDatabase):
   
    connection = None
    print(f" [CONSUMER INFO] Intentando conectar a RabbitMQ en: {RABBITMQ_URL}")

    while True:
        try:
            connection = await aio_pika.connect_robust(RABBITMQ_URL)
            print(f" [CONSUMER SUCCESS] Conectado a RabbitMQ en {RABBITMQ_URL}!")

            async with connection:
                channel = await connection.channel()

               
                await channel.set_qos(prefetch_count=10)

                exchange = await channel.declare_exchange(
                    EXCHANGE_NAME, aio_pika.ExchangeType.DIRECT, durable=True
                )

                queue = await channel.declare_queue(QUEUE_NAME, durable=True)

                for r_key in ROUTING_KEYS_TO_BIND:
                    await queue.bind(exchange, routing_key=r_key)
                    print(f" [CONSUMER INFO] Cola '{QUEUE_NAME}' vinculada a exchange '{EXCHANGE_NAME}' con routing key '{r_key}'")

                print(f" [CONSUMER INFO] Esperando mensajes en la cola '{QUEUE_NAME}'. Para salir, detén la aplicación.")

                async with queue.iterator() as queue_iter:
                    async for message in queue_iter:
                     
                        async with message.process(ignore_processed=False): 
                            try:
                                body_str = message.body.decode()
                                print(f"\n [CONSUMER INFO] Mensaje recibido! Routing Key: '{message.routing_key}', Body: '{body_str}'")
                                
                                data = json.loads(body_str)
                                event_type = data.get("event_type") 
                                payload = data.get("payload", {})  

                                if message.routing_key == "user.username.updated" and event_type == "USERNAME_UPDATED":
                                    await process_username_updated(payload, db)
                                elif message.routing_key == "user.email.updated" and event_type == "USER_EMAIL_UPDATED":
                                    await process_email_updated(payload, db)
                                elif message.routing_key == "user.deleted" and event_type == "USER_DELETED":
                                    await process_user_deleted(payload, db)
                                else:
                                    print(f" [CONSUMER WARNING] Evento o routing key no reconocido/manejado: type='{event_type}', key='{message.routing_key}'. Mensaje ignorado (ack).")

                            except json.JSONDecodeError:
                                print(f" [CONSUMER ERROR] Fallo al decodificar cuerpo del mensaje JSON: '{message.body.decode()[:100]}...' El mensaje será rechazado (nack) y no reencolado.")
                               
                            except Exception as e:
                                print(f" [CONSUMER ERROR] Error crítico procesando mensaje (routing_key: {message.routing_key}): {e}. El mensaje podría ser reencolado.")
                                raise 
        
        except aio_pika.exceptions.AMQPConnectionError as e:
            print(f" [CONSUMER ERROR] Error de conexión con RabbitMQ (AMQPConnectionError): {e}. Reintentando en 5 segundos...")
        except ConnectionRefusedError as e:
            print(f" [CONSUMER ERROR] Conexión con RabbitMQ rechazada (ConnectionRefusedError): {e}. ¿Está RabbitMQ corriendo y accesible? Reintentando en 5 segundos...")
        except Exception as e:
            print(f" [CONSUMER ERROR] Error inesperado en la configuración del consumidor de RabbitMQ: {e}. Reintentando en 5 segundos...")
        
        finally:
            if connection and not connection.is_closed:
                try:
                    print(" [CONSUMER INFO] Cerrando conexión actual de RabbitMQ antes de reintentar o salir...")
                    await connection.close()
                except Exception as e_close:
                    print(f" [CONSUMER WARNING] Error al cerrar la conexión de RabbitMQ: {e_close}")
            print(" [CONSUMER INFO] Esperando 5 segundos antes del próximo intento de conexión a RabbitMQ...")
            await asyncio.sleep(5)