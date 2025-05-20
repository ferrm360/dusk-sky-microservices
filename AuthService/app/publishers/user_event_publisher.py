import aio_pika
import json
from app.config.settings import settings

class UserEventPublisher:
    def __init__(self):
        self.rabbitmq_url = settings.RABBITMQ_URL_AUTH
        self.exchange_name = "user_events_exchange"

    async def publish(self, event_type: str, routing_key: str, payload: dict):
        try:
            connection = await aio_pika.connect_robust(self.rabbitmq_url)
            async with connection:
                channel = await connection.channel()
                exchange = await channel.declare_exchange(
                    self.exchange_name, aio_pika.ExchangeType.DIRECT, durable=True
                )
                message_body = json.dumps({
                    "event_type": event_type,
                    "payload": payload
                }).encode()

                message = aio_pika.Message(body=message_body)
                await exchange.publish(message, routing_key=routing_key)
                print(f"[PUBLISHER] Evento '{event_type}' enviado a '{routing_key}'")
        except Exception as e:
            print(f"[PUBLISHER ERROR] No se pudo publicar '{event_type}': {e}")
