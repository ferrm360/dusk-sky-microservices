import os
from dotenv import load_dotenv
from motor.motor_asyncio import AsyncIOMotorClient

load_dotenv()

MONGO_REVIEW_URI = os.getenv("MONGO_URI", "mongodb://localhost:27017")
MONGO_REVIEW_DB = os.getenv("MONGO_DB", "review_service_db")

client = AsyncIOMotorClient(MONGO_REVIEW_URI)
db = client[MONGO_REVIEW_DB]
