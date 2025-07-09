import aio_pika
import json
from app.config.settings import settings

RABBITMQ_URL = settings.RABBITMQ_URL_USER
EXCHANGE_NAME = "user_events_exchange"

async def publish_user_event(event_type: str, routing_key: str, payload: dict):
    try:
        print(f"[PUBLISHER] Conectando a RabbitMQ en {RABBITMQ_URL}...")
        connection = await aio_pika.connect_robust(RABBITMQ_URL)
        print("[PUBLISHER] Conexión establecida")

        async with connection:
            print("[PUBLISHER] Abriendo canal...")
            channel = await connection.channel()

            print("[PUBLISHER] Declarando exchange...")
            exchange = await channel.declare_exchange(EXCHANGE_NAME, aio_pika.ExchangeType.DIRECT, durable=True)

            message_body = json.dumps({
                "event_type": event_type,
                "payload": payload
            }).encode()

            message = aio_pika.Message(body=message_body)

            print(f"[PUBLISHER] Publicando mensaje: {message_body.decode()}")
            await exchange.publish(message, routing_key=routing_key)
            print(f"[PUBLISHER SUCCESS] Evento '{event_type}' enviado con routing key '{routing_key}'")

    except Exception as e:
        print(f"[PUBLISHER ERROR] Error publicando evento '{event_type}': {e}")
