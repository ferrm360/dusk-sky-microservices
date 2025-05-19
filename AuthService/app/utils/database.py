from motor.motor_asyncio import AsyncIOMotorClient, AsyncIOMotorDatabase
from ..config import settings

client: AsyncIOMotorClient = None

async def get_database() -> AsyncIOMotorDatabase:
    global client
    if client is None:
        client = AsyncIOMotorClient(settings.settings.MONGODB_URI_AUTH)
    db = client[settings.settings.MONGODB_NAME_AUTH]
    return db