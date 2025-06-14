from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routes import review_routes

app = FastAPI(title="Review Service")

# Middleware CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Incluir rutas
app.include_router(review_routes.router, prefix="/reviews", tags=["Reviews"])
