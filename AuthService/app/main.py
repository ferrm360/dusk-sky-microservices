# app/main.py

from fastapi import FastAPI
import asyncio

from .routes import auth 
from app.config import settings 

from .consumers.user_event_consumer import consume_user_events
from .utils.database import get_database

app = FastAPI(
    title="AuthService",
    description="Microservicio para la autenticación.",
    version="1.0.0"
)

@app.on_event("startup")
async def startup_event():
    print("INFO:     Iniciando AuthService...")

    try:
        db_for_consumer = await get_database()
        print("INFO:     Conexión a la base de datos establecida para el consumidor.")
        
        print("INFO:     Iniciando el consumidor de eventos de RabbitMQ...")
        asyncio.create_task(consume_user_events(db_for_consumer))
        print("INFO:     Tarea del consumidor de RabbitMQ creada y corriendo en segundo plano.")
        
    except Exception as e:
      
        print(f"ERROR:    Error durante el evento de startup al configurar el consumidor: {e}")

@app.on_event("shutdown")
async def shutdown_event():
    print("INFO:     Apagando AuthService...")

    print("INFO:     AuthService apagado.")

# --- Incluir tus routers ---
app.include_router(auth.router, prefix="/auth", tags=["Authentication"])

@app.get("/health", tags=["Health Check"])
async def health_check():
    return {"status": "ok", "message": "AuthService is healthy!"}