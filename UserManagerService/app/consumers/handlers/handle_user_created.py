from app.controllers.user_profile_controller import create_profile_with_custom_id

async def handle(payload, db):
    user_id = payload.get("user_id")
    if not user_id:
        print("[ERROR] Falta user_id en evento USER_CREATED")
        return

    existing = await db["user_profiles"].find_one({"_id": user_id})
    if existing:
        print(f"[WARNING] Perfil con user_id '{user_id}' ya existe. Se omite creación.")
        return

    await create_profile_with_custom_id(user_id, db)
    print(f"[SUCCESS] Perfil creado correctamente para user_id: {user_id}")
