from app.controllers import auth_controller

async def handle(payload, db):
    user_id = payload.get("user_id")

    if not user_id:
        print(f"[ERROR] Payload inválido en USER_DELETED: {payload}")
        return

    print(f"[INFO] USER_DELETED → user_id: {user_id}")
    await auth_controller.delete_user(user_id=user_id, db=db)
    print(f"[SUCCESS] Usuario {user_id} eliminado")
