from app.controllers import auth_controller

async def handle(payload, db):
    user_id = payload.get("user_id")
    new_email = payload.get("new_email")

    if not user_id or not new_email:
        print(f"[ERROR] Payload inválido en USER_EMAIL_UPDATED: {payload}")
        return

    print(f"[INFO] USER_EMAIL_UPDATED → user_id: {user_id}, new_email: {new_email}")
    await auth_controller.update_email(user_id=user_id, new_email=new_email, db=db)
    print(f"[SUCCESS] Email actualizado para {user_id}")
