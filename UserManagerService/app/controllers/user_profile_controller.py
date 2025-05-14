from motor.motor_asyncio import AsyncIOMotorDatabase
from bson import ObjectId
from datetime import datetime
from fastapi.encoders import jsonable_encoder

from app.models.user_profile_model import (
    UserProfileCreate,
    UserProfileUpdate,
    UserProfileInDB
)

COLLECTION_NAME = "user_profiles"

# 🔍 Obtener perfil por user_id
async def get_profile_by_user_id(user_id: str, db: AsyncIOMotorDatabase) -> UserProfileInDB | None:
    doc = await db[COLLECTION_NAME].find_one({"user_id": user_id})
    if doc:
        doc["_id"] = str(doc["_id"])  # Convertir ObjectId a str
        return UserProfileInDB(**doc)
    return None

# 🆕 Crear perfil
async def create_profile(profile_data: UserProfileCreate, db: AsyncIOMotorDatabase) -> UserProfileInDB:
    document = jsonable_encoder(profile_data, by_alias=True)  # Convierte HttpUrl, datetime, etc.
    insert_result = await db[COLLECTION_NAME].insert_one(document)

    new_doc = await db[COLLECTION_NAME].find_one({"_id": insert_result.inserted_id})
    new_doc["_id"] = str(new_doc["_id"])
    return UserProfileInDB(**new_doc)

# ✏️ Actualizar perfil
async def update_profile(user_id: str, update_data: UserProfileUpdate, db: AsyncIOMotorDatabase) -> UserProfileInDB | None:
    update_doc = jsonable_encoder(update_data, exclude_unset=True)  # Solo campos enviados
    update_doc["updated_at"] = datetime.utcnow()

    result = await db[COLLECTION_NAME].update_one(
        {"user_id": user_id},
        {"$set": update_doc}
    )

    if result.modified_count == 0:
        return None

    updated_doc = await db[COLLECTION_NAME].find_one({"user_id": user_id})
    updated_doc["_id"] = str(updated_doc["_id"])
    return UserProfileInDB(**updated_doc)

# ❌ Eliminar perfil
async def delete_profile_by_user_id(user_id: str, db: AsyncIOMotorDatabase) -> bool:
    result = await db[COLLECTION_NAME].delete_one({"user_id": user_id})
    return result.deleted_count > 0
