from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routes import sales_routes

app = FastAPI(title="Sales Service")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)



app.include_router(sales_routes.router, prefix="/sales", tags=["Sales"])
