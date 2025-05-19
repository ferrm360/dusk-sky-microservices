from app.consumers.handlers import handle_user_created

handlers = {
    "user.created": ("USER_CREATED", handle_user_created.handle),
}
