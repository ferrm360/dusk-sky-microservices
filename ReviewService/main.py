from fastapi import FastAPI
from routes import review_routes

app = FastAPI(title="Review Service")

app.include_router(review_routes.router, prefix="/reviews", tags=["Reviews"])
