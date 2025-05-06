from fastapi import FastAPI
from .routes import auth
from .config import settings

app = FastAPI(
    title="AuthService",
    description="Microservicio para la autenticación.",
    version="1.0.0"
)

app.include_router(auth.router, prefix="/auth")

@app.get("/health")
async def health_check():
    return {"status": "ok"}