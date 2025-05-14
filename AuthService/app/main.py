# app/main.py

from fastapi import FastAPI
import asyncio # Para crear tareas en segundo plano

# --- Tus importaciones existentes ---
from .routes import auth # Asumiendo que es app.routes.auth
from .config import settings # Asumiendo que es app.config.settings

# --- Nuevas importaciones para el consumidor y la base de datos ---
# Ajusta estas rutas si tus archivos están en otro lugar dentro de 'app'
from .consumers.user_event_consumer import consume_user_events
from .utils.database import get_database

# --- Tu instancia de FastAPI ---
app = FastAPI(
    title="AuthService",
    description="Microservicio para la autenticación.",
    version="1.0.0"
    # Puedes agregar aquí más metadatos si lo deseas
)

# --- Evento de Inicio (Startup) ---
@app.on_event("startup")
async def startup_event():
    print("INFO:     Iniciando AuthService...")
    
    # Obtener una instancia de la base de datos para el consumidor
    # Usamos tu función get_database() directamente, como discutimos.
    try:
        db_for_consumer = await get_database()
        print("INFO:     Conexión a la base de datos establecida para el consumidor.")
        
        # Iniciar el consumidor de RabbitMQ como una tarea en segundo plano
        print("INFO:     Iniciando el consumidor de eventos de RabbitMQ...")
        asyncio.create_task(consume_user_events(db_for_consumer))
        print("INFO:     Tarea del consumidor de RabbitMQ creada y corriendo en segundo plano.")
        
    except Exception as e:
        # Si hay un error al obtener la BD o al crear la tarea, lo registramos.
        # Esto podría impedir que el consumidor se inicie.
        print(f"ERROR:    Error durante el evento de startup al configurar el consumidor: {e}")
        # Podrías decidir si la aplicación debe fallar aquí o continuar sin el consumidor.

# --- Evento de Apagado (Shutdown) ---
@app.on_event("shutdown")
async def shutdown_event():
    print("INFO:     Apagando AuthService...")
    # Aquí quitamos la llamada a close_mongo_connection
    # Si implementas close_mongo_connection en el futuro, puedes volver a agregarla aquí.
    print("INFO:     AuthService apagado.")

# --- Incluir tus routers ---
app.include_router(auth.router, prefix="/auth", tags=["Authentication"])

# --- Ruta de Health Check (ya la tenías) ---
@app.get("/health", tags=["Health Check"])
async def health_check():
    return {"status": "ok", "message": "AuthService is healthy!"}