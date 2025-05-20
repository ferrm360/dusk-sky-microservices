from fastapi import HTTPException
from app.Database.mongodb import db
from app.schemas.friendship_schema import FriendshipRequest, FriendshipResponse
from uuid import uuid4
from datetime import datetime
from app.Database.mongodb import db
from app.utils.serializers import serialize_doc

collection = db["friendships"]

def normalize_ids(sender_id: str, receiver_id: str) -> tuple[str, str]:
    return tuple(sorted([sender_id, receiver_id]))

async def send_request(sender_id: str, receiver_id: str):
    if sender_id == receiver_id:
        raise HTTPException(status_code=400, detail="Cannot send request to yourself")

    user1, user2 = normalize_ids(sender_id, receiver_id)

    # Verifica si ya existe
    existing = await collection.find_one({
        "$or": [
            {"sender_id": user1, "receiver_id": user2},
            {"sender_id": user2, "receiver_id": user1}
        ]
    })
    if existing:
        raise HTTPException(status_code=400, detail="Friendship request already exists")

    new_friendship = {
        "id": str(uuid4()),
        "sender_id": sender_id,
        "receiver_id": receiver_id,
        "status": "pending",
        "requested_at": datetime.utcnow()
    }

    await collection.insert_one(new_friendship)
    return {"id": new_friendship["id"], "message": "Friend request sent"}


async def accept_request(request_id: str):
    result = await collection.update_one(
        {"id": request_id},
        {"$set": {"status": "accepted"}}
    )
    if result.matched_count == 0:
        raise HTTPException(status_code=404, detail="Friendship request not found")
    return {"message": "Friendship accepted"}


async def reject_request(request_id: str):
    result = await collection.delete_one({"id": request_id})
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Friendship request not found")
    return {"message": "Friend request rejected"}


async def get_friends(user_id: str):
    query = {
        "$and": [
            {"status": "accepted"},
            {"$or": [{"sender_id": user_id}, {"receiver_id": user_id}]}
        ]
    }
    cursor = db.friendships.find(query)
    friendships = await cursor.to_list(length=100)
    return [serialize_doc(friend) for friend in friendships]


async def get_pending_requests(user_id: str):
    query = {"status": "pending", "receiver_id": user_id}
    cursor = db.friendships.find(query)
    requests = await cursor.to_list(length=100)
    return [serialize_doc(req) for req in requests]