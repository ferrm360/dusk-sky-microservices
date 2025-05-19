from fastapi import FastAPI
from app.routes.friendship_routes import router as friendship_router

app = FastAPI()

app.include_router(friendship_router)