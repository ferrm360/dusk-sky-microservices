# auth_controller.py
from datetime import datetime, timedelta
from fastapi import HTTPException
from motor.motor_asyncio import AsyncIOMotorDatabase
from ..publishers.user_event_publisher import UserEventPublisher
from app.config import settings
from app.models import LoginRequest
from ..models import user, token
from ..utils import security
from bson import ObjectId

async def register_user(user_in: user.UserCreate, db: AsyncIOMotorDatabase) -> user.UserInDB:
    if not user_in.username or not user_in.email or not user_in.password:
        raise ValueError("Username, email, and password are required")
      
    users_collection = db.users
    user_secrets_collection = db.user_secrets

    existing_user = await users_collection.find_one({"$or": [{"username": user_in.username}, {"email": user_in.email}]})
    if existing_user:
        raise ValueError("Username or email already taken")

    hashed_password, password_salt = security.hash_password(user_in.password)
    new_user_data = user.User(
        username=user_in.username,
        email=user_in.email,
        created_at=datetime.utcnow()
    ).dict()

    result = await users_collection.insert_one(new_user_data)
    new_user_id = result.inserted_id

    await user_secrets_collection.insert_one({
        "user_id": str(new_user_id),
        "password_hash": hashed_password,
        "password_salt": password_salt,
    })

    publisher = UserEventPublisher()
    await publisher.publish(
        event_type="USER_CREATED",
        routing_key="user.created",  # <- lo escucha UserManagerService
        payload={"user_id": str(new_user_id)}
    )


    new_user = await users_collection.find_one({"_id": new_user_id})
    new_user["_id"] = str(new_user["_id"])
    return user.UserInDB(**new_user)

async def login_for_access_token(login_request: LoginRequest.LoginRequest, db: AsyncIOMotorDatabase) -> token.Token:
    if not login_request.username or not login_request.password:
        raise ValueError("Username and password are required")
     
    users_collection = db.users
    user_secrets_collection = db.user_secrets

    user = await users_collection.find_one({"username": login_request.username})
    if not user:
        raise ValueError("Incorrect username or password")

    user_secret = await user_secrets_collection.find_one({"user_id": str(user["_id"])})
    if not user_secret or not security.verify_password(login_request.password, user_secret["password_hash"]):
        raise ValueError("Incorrect username or password")

    access_token_expires = timedelta(minutes=settings.settings.JWT_ACCESS_TOKEN_EXPIRE_MINUTES_AUTH)

    user_data_for_token = {
        "_id": str(user["_id"]),
        "username": user["username"],
        "email": user["email"],
        "role": user.get("role", "player"),
        "status": user.get("status", "active"),
        "created_at": user["created_at"].isoformat()
    }

    access_token = security.create_access_token(
        subject=str(user["_id"]),
        user_data=user_data_for_token,
        expires_delta=access_token_expires
    )

    return {"access_token": access_token, "token_type": "bearer"}


async def delete_user(user_id: str, db: AsyncIOMotorDatabase):
    users_collection = db.users
    user_secrets_collection = db.user_secrets
    
    user = await users_collection.find_one({"_id": ObjectId(user_id)})
    if not user:
        raise ValueError("User not found")
    
    await users_collection.update_one({"_id": ObjectId(user_id)}, {"$set": {"status": "deleted"}})

    return {"message": "User account deleted successfully"}


async def change_password(user_id: str, current_password: str, new_password: str, db: AsyncIOMotorDatabase):
    users_collection = db.users
    user_secrets_collection = db.user_secrets

    user = await users_collection.find_one({"_id": ObjectId(user_id)})
    if not user:
        raise ValueError("User not found")

    user_secret = await user_secrets_collection.find_one({"user_id": user_id})
    if not user_secret or not security.verify_password(current_password, user_secret["password_hash"]):
        raise ValueError("Incorrect password")

    hashed_password, password_salt = security.hash_password(new_password)

    await user_secrets_collection.update_one(
        {"user_id": user_id},
        {"$set": {"password_hash": hashed_password, "password_salt": password_salt}}
    )
    
    return {"message": "Password changed successfully"}


async def update_username(user_id: str, new_username: str, db: AsyncIOMotorDatabase):
    users_collection = db.users

    existing_user = await users_collection.find_one({"username": new_username})
    if existing_user:
        raise ValueError("Username already taken")

    updated_user = await users_collection.update_one(
        {"_id": ObjectId(user_id)},
        {"$set": {"username": new_username}}
    )
    
    if updated_user.matched_count == 0:
        raise ValueError("User not found")

    return {"message": "Username updated successfully"}


async def update_email(user_id: str, new_email: str, db: AsyncIOMotorDatabase):
    users_collection = db.users

    existing_user = await users_collection.find_one({"email": new_email})
    if existing_user:
        raise ValueError("Email already taken")

    updated_user = await users_collection.update_one(
        {"_id": ObjectId(user_id)},
        {"$set": {"email": new_email}}
    )

    if updated_user.matched_count == 0:
        raise ValueError("User not found")

    return {"message": "Email updated successfully"}

async def search_users_by_username(query: str, db: AsyncIOMotorDatabase):
    users_collection = db.users
    regex = {"$regex": query, "$options": "i"}  # búsqueda insensible a mayúsculas

    cursor = users_collection.find({"username": regex})

    users = []
    async for user in cursor:
        users.append({
            "id": str(user["_id"]),
            "username": user["username"],
            "email": user["email"],
            "role": user.get("role", "player"),
            "status": user.get("status", "active")
        })
    return users

async def get_user_by_id(user_id: str, db: AsyncIOMotorDatabase):
    users_collection = db.users
    user = await users_collection.find_one({"_id": ObjectId(user_id)})
    if not user:
        raise ValueError("User not found")

    user["_id"] = str(user["_id"])
    return {
        "id": user["_id"],
        "username": user["username"],
        "email": user["email"],
        "role": user.get("role", "player"),
        "status": user.get("status", "active"),
        "created_at": user["created_at"]
    }

async def promote_user(user_id: str, db: AsyncIOMotorDatabase) -> user.UserInDB:
    users_collection = db.users

    # Convertir el user_id de str a ObjectId
    user_oid = ObjectId(user_id) # Renamed variable

    # Verificar si el usuario existe
    user_data = await users_collection.find_one({"_id": user_oid}) # Renamed variable
    if not user_data:
        raise HTTPException(status_code=404, detail="User not found")

    # Verificar si ya es moderador
    if user_data.get("role") == "moderator":
        raise HTTPException(status_code=400, detail="User is already a moderator")

    # Promover a moderador
    result = await users_collection.update_one(
        {"_id": user_oid},
        {"$set": {"role": "moderator"}}
    )

    if result.matched_count == 0:
        raise HTTPException(status_code=400, detail="Failed to promote user")

    # Obtener el usuario actualizado
    updated_user_data = await users_collection.find_one({"_id": user_oid}) # Renamed variable

    # Asegurarnos de convertir el _id de ObjectId a string
    if updated_user_data:
        updated_user_data["_id"] = str(updated_user_data["_id"])
    
    # Devuelve solo lo necesario sin el ObjectId de MongoDB
    return user.UserInDB(**updated_user_data) # Correctly using the imported 'user' module


async def demote_user(user_id: str, db: AsyncIOMotorDatabase) -> user.UserInDB:
    users_collection = db.users

    user_oid = ObjectId(user_id) 

    user_data = await users_collection.find_one({"_id": user_oid}) # Renamed variable
    if not user_data:
        raise HTTPException(status_code=404, detail="User not found")

    if user_data.get("role") == "player":
        raise HTTPException(status_code=400, detail="User is already a player")

    result = await users_collection.update_one(
        {"_id": user_oid},
        {"$set": {"role": "player"}}
    )

    if result.matched_count == 0:
        raise HTTPException(status_code=400, detail="Failed to demote user")

    updated_user_data = await users_collection.find_one({"_id": user_oid}) # Renamed variable

    if updated_user_data:
        updated_user_data["_id"] = str(updated_user_data["_id"])
    
    return user.UserInDB(**updated_user_data) 