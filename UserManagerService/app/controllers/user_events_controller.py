from motor.motor_asyncio import AsyncIOMotorDatabase
from app.events.publishers.user_events_publisher import publish_user_event


async def update_username(user_id: str, new_username: str, db: AsyncIOMotorDatabase) -> bool:
    try:
        await publish_user_event("USERNAME_UPDATED", "user.username.updated", {
            "user_id": user_id,
            "new_username": new_username
        })
        return True
    except Exception as e:
        print(f"[ERROR] Fallo al publicar evento USERNAME_UPDATED: {e}")
        return False


async def update_email(user_id: str, new_email: str, db: AsyncIOMotorDatabase) -> bool:
    result = await db["user_profiles"].update_one(
        {"user_id": user_id},
        {"$set": {"email": new_email}}  
    )
    if result.modified_count:
        await publish_user_event("USER_EMAIL_UPDATED", "user.email.updated", {
            "user_id": user_id,
            "new_email": new_email
        })
        return True
    return False


async def change_password(user_id: str, new_password_hash: str, db: AsyncIOMotorDatabase) -> bool:
    result = await db["user_profiles"].update_one(
        {"user_id": user_id},
        {"$set": {"password": new_password_hash}}  
    )
    if result.modified_count:
        await publish_user_event("USER_CHANGE_PASSWORD", "user.changePassword", {
            "user_id": user_id,
            "new_password_hash": new_password_hash
        })
        return True
    return False


async def delete_user(user_id: str, db: AsyncIOMotorDatabase) -> bool:
    result = await db["user_profiles"].delete_one({"user_id": user_id})
    if result.deleted_count:
        await publish_user_event("USER_DELETED", "user.deleted", {
            "user_id": user_id
        })
        return True
    return False
