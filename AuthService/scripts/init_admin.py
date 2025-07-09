import asyncio
from bson import ObjectId
from app.config import settings
from app.controllers.auth_controller import register_user
from app.models.user import UserCreate
from app.utils.database import get_database

async def init_admin():
    db = await get_database()
    users_collection = db.users

    existing_admin = await users_collection.find_one({"username": "admin"})
    if existing_admin:
        print("✅ Admin user already exists.")
        return

    admin_email = settings.settings.ADMIN_EMAIL
    admin_password = settings.settings.ADMIN_PASSWORD

    admin_data = UserCreate(
        username="admin",
        email=admin_email,
        password=admin_password
    )

    print("🛠️ Creating admin user...")

    created_user = await register_user(admin_data, db)

    await users_collection.update_one(
        {"_id": ObjectId(created_user.id)},
        {"$set": {"role": "admin"}}
    )

    print(f"✅ Admin user created with ID: {created_user.id}")

if __name__ == "__main__":
    asyncio.run(init_admin())
