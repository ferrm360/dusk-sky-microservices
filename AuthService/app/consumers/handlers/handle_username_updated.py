from app.controllers import auth_controller

async def handle(payload, db):
    user_id = payload.get("user_id")
    new_username = payload.get("new_username")

    if not user_id or new_username is None:
        print(f"[ERROR] Payload inválido en USERNAME_UPDATED: {payload}")
        return

    print(f"[INFO] USERNAME_UPDATED → user_id: {user_id}, new_username: {new_username}")
    await auth_controller.update_username(user_id=user_id, new_username=new_username, db=db)
    print(f"[SUCCESS] Username actualizado para {user_id}")
