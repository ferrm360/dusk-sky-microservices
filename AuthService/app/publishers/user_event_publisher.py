import aio_pika
import asyncio
import json
from app.config.settings import settings

class UserEventPublisher:
    def __init__(self, retries=10, delay=5):  # más intentos y más espera
        self.rabbitmq_url = settings.RABBITMQ_URL_AUTH
        self.exchange_name = "user_events_exchange"
        self.retries = retries
        self.delay = delay

    async def publish(self, event_type: str, routing_key: str, payload: dict):
        message_body = json.dumps({
            "event_type": event_type,
            "payload": payload
        }).encode()

        for attempt in range(1, self.retries + 1):
            try:
                # ✅ Previene el error "no current event loop"
                try:
                    asyncio.get_running_loop()
                except RuntimeError:
                    asyncio.set_event_loop(asyncio.new_event_loop())

                connection = await aio_pika.connect_robust(self.rabbitmq_url)
                async with connection:
                    channel = await connection.channel()
                    exchange = await channel.declare_exchange(
                        self.exchange_name, aio_pika.ExchangeType.DIRECT, durable=True
                    )

                    message = aio_pika.Message(
                        body=message_body,
                        delivery_mode=aio_pika.DeliveryMode.PERSISTENT
                    )

                    await exchange.publish(message, routing_key=routing_key)
                    print(f"📤 Evento '{event_type}' publicado exitosamente en intento {attempt}")
                    return

            except Exception as e:
                print(f"[PUBLISHER ERROR] Intento {attempt} falló: {e}")
                if attempt < self.retries:
                    await asyncio.sleep(self.delay)
                else:
                    print(f"❌ Fallaron todos los intentos de publicar '{event_type}'")
