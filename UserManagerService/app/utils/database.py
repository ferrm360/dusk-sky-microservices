# app/utils/database.py
from motor.motor_asyncio import AsyncIOMotorClient, AsyncIOMotorDatabase
from typing import Optional
from app.config.settings import settings

_mongo_client_user_manager: Optional[AsyncIOMotorClient] = None

async def get_user_manager_database() -> AsyncIOMotorDatabase:
    global _mongo_client_user_manager

    if _mongo_client_user_manager is None:
        print(f"INFO: [UserManagerDB] Conectando a {settings.MONGODB_URI}")
        _mongo_client_user_manager = AsyncIOMotorClient(settings.MONGODB_URI)
    else:
        print(f"INFO: [UserManagerDB] Reutilizando conexión Mongo existente.")

    return _mongo_client_user_manager[settings.MONGODB_NAME]

async def close_user_manager_mongo_connection():
    global _mongo_client_user_manager
    if _mongo_client_user_manager:
        print("INFO: [UserManagerDB] Cerrando conexión MongoDB...")
        _mongo_client_user_manager.close()
        _mongo_client_user_manager = None
