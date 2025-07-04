from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routes.friendship_routes import router as friendship_router

app = FastAPI(title="Friendship Service")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], 
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(friendship_router)
