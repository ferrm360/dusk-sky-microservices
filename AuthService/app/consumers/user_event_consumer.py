import asyncio
import json
import aio_pika
from motor.motor_asyncio import AsyncIOMotorDatabase
from app.config.settings import settings
from app.consumers.handlers import handlers

RABBITMQ_URL = settings.RABBITMQ_URL_AUTH
EXCHANGE_NAME = 'user_events_exchange'
QUEUE_NAME = 'auth_service_user_updates_queue'

async def consume_user_events(db: AsyncIOMotorDatabase):
    connection = None
    print(f"[CONSUMER] Intentando conectar a RabbitMQ en: {RABBITMQ_URL}")

    while True:
        try:
            connection = await aio_pika.connect_robust(RABBITMQ_URL)
            print(f"[CONSUMER] Conectado a RabbitMQ en {RABBITMQ_URL}")

            async with connection:
                channel = await connection.channel()
                await channel.set_qos(prefetch_count=10)

                exchange = await channel.declare_exchange(
                    EXCHANGE_NAME, aio_pika.ExchangeType.DIRECT, durable=True
                )

                queue = await channel.declare_queue(QUEUE_NAME, durable=True)

                for routing_key in handlers:
                    await queue.bind(exchange, routing_key=routing_key)
                    print(f"[BIND] Cola '{QUEUE_NAME}' vinculada a routing key '{routing_key}'")

                print(f"[CONSUMER] Esperando mensajes en la cola '{QUEUE_NAME}'...")

                async with queue.iterator() as queue_iter:
                    async for message in queue_iter:
                        async with message.process(ignore_processed=False):
                            try:
                                body_str = message.body.decode()
                                print(f"\n[RECEIVED] '{message.routing_key}': {body_str}")

                                data = json.loads(body_str)
                                event_type = data.get("event_type")
                                payload = data.get("payload", {})

                                expected_type, handler_fn = handlers.get(message.routing_key, (None, None))

                                if handler_fn and event_type == expected_type:
                                    await handler_fn(payload, db)
                                else:
                                    print(f"[WARNING] Evento no manejado o tipo inesperado: '{event_type}' con key '{message.routing_key}'")

                            except json.JSONDecodeError:
                                print(f"[ERROR] JSON inválido: {message.body.decode()[:100]}")
                            except Exception as e:
                                print(f"[ERROR] Fallo crítico al procesar mensaje: {e}")
                                raise

        except aio_pika.exceptions.AMQPConnectionError as e:
            print(f"[ERROR] AMQPConnectionError: {e} → Reintentando en 5s")
        except ConnectionRefusedError as e:
            print(f"[ERROR] ConnectionRefusedError: {e} → ¿Está RabbitMQ activo? Reintentando...")
        except Exception as e:
            print(f"[ERROR] Fallo inesperado: {e} → Reintentando en 5s")
        finally:
            if connection and not connection.is_closed:
                try:
                    print("[CLOSE] Cerrando conexión a RabbitMQ...")
                    await connection.close()
                except Exception as e_close:
                    print(f"[CLOSE ERROR] No se pudo cerrar la conexión: {e_close}")
            await asyncio.sleep(5)
