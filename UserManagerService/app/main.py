from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles
from pathlib import Path

from app.routes import user_profile
from app.utils.database import close_user_manager_mongo_connection

app = FastAPI(
    title="UserManagerService",
    version="1.0.0",
    description="Microservicio para gestionar perfiles públicos de usuarios."
)

STATIC_DIR = Path(__file__).parent / "static_user_content"
app.mount("/static", StaticFiles(directory=STATIC_DIR), name="static")

app.include_router(user_profile.router)

@app.on_event("shutdown")
async def shutdown_mongo():
    await close_user_manager_mongo_connection()

@app.get("/")
async def root():
    return {
        "message": "UserManagerService corriendo con FastAPI + MongoDB.",
        "ruta_perfiles": "/profiles/{user_id}",
        "static_example": "/static/avatars/default_avatar.jpg"
    }
