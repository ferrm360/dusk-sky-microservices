from app.consumers.handlers import (
    handle_username_updated,
    handle_email_updated,
    handle_user_deleted,
)

handlers = {
    "user.username.updated": ("USERNAME_UPDATED", handle_username_updated.handle),
    "user.email.updated": ("USER_EMAIL_UPDATED", handle_email_updated.handle),
    "user.deleted": ("USER_DELETED", handle_user_deleted.handle),
}
