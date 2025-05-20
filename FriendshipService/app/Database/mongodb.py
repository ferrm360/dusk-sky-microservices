import os
from motor.motor_asyncio import AsyncIOMotorClient

# Obtener variables de entorno
MONGO_URI = os.getenv("MONGO_URI", "mongodb://localhost:27017")
MONGO_DB_NAME = os.getenv("MONGO_DB", "friendship_db")

# Conexión al cliente de Mongo
client = AsyncIOMotorClient(MONGO_URI)
db = client[MONGO_DB_NAME]
