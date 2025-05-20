from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles
from pathlib import Path
import asyncio

from app.routes import user_profile
from app.utils.database import close_user_manager_mongo_connection
from app.utils.database import get_database
from app.consumers.user_event_consumer import consume_user_events

print("✅ main.py: archivo ejecutado")

app = FastAPI(
    title="UserManagerService",
    version="1.0.0",
    description="Microservicio para gestionar perfiles públicos de usuarios."
)

STATIC_DIR = Path(__file__).parent / "static_user_content"
app.mount("/static", StaticFiles(directory=STATIC_DIR), name="static")

@app.on_event("startup")
async def startup_event():

    print("INFO:     Iniciando UserManagerService...")

    try:
        db = await get_database()
        print("INFO:     Conexión a MongoDB establecida para consumidor.")
        
        print("INFO:     Iniciando consumidor de RabbitMQ...")
        asyncio.create_task(consume_user_events(db))
        print("INFO:     Consumidor RabbitMQ corriendo en segundo plano.")
    except Exception as e:
        print(f"ERROR:    Falló el inicio del consumidor: {e}")


#@app.on_event("shutdown")
#async def shutdown_mongo():
 #   await close_user_manager_mongo_connection()

app.include_router(user_profile.router)

@app.get("/")
async def root():
    return {
        "message": "UserManagerService corriendo con FastAPI + MongoDB.",
        "ruta_perfiles": "/profiles/{user_id}",
        "static_example": "/static/avatars/default_avatar.jpg"
    }
