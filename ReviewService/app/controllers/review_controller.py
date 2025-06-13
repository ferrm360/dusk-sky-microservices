from typing import List, Optional
from bson import ObjectId
from app.database.database import db

collection = db["reviews"]

def serialize_review(doc):
    doc["id"] = str(doc["_id"])
    del doc["_id"]
    return doc

# Crear reseña
async def create_review(review):
    doc = review.model_dump()
    doc["createdAt"] = review.createdAt.isoformat()
    result = await collection.insert_one(doc)
    return {"id": str(result.inserted_id)}

# Eliminar reseña si userId coincide
async def delete_review(review_id: str, user_id: str):
    result = await collection.delete_one({
        "_id": ObjectId(review_id),
        "userId": user_id
    })
    return result.deleted_count > 0

# Dar like
async def like_review(review_id: str, user_id: str):
    result = await collection.update_one(
        {"_id": ObjectId(review_id), "likedBy": {"$ne": user_id}},
        {"$push": {"likedBy": user_id}, "$inc": {"likes": 1}}
    )
    return result.modified_count > 0

# Quitar like
async def unlike_review(review_id: str, user_id: str):
    result = await collection.update_one(
        {"_id": ObjectId(review_id), "likedBy": user_id},
        {"$pull": {"likedBy": user_id}, "$inc": {"likes": -1}}
    )
    return result.modified_count > 0

# Obtener reseñas ordenadas, con filtro opcional por juego
async def get_reviews_sorted(field: str, direction: int, limit: int = 10, game_id: Optional[str] = None):
    query = {"gameId": game_id} if game_id else {}
    cursor = collection.find(query).sort(field, direction).limit(limit)
    return [serialize_review(doc) async for doc in cursor]


# Reseñas de amigos, con filtro por juego si aplica
async def get_reviews_from_users(user_ids: List[str], limit: int = 10, game_id: Optional[str] = None):
    query = {"userId": {"$in": user_ids}}
    if game_id:
        query["gameId"] = game_id
    cursor = collection.find(query).sort("createdAt", -1).limit(limit)
    return [serialize_review(doc) async for doc in cursor]

async def get_review_by_id(review_id: str):
    doc = await collection.find_one({"_id": ObjectId(review_id)})
    if not doc:
        return None
    return serialize_review(doc)

