from pydantic import BaseSettings

class Settings(BaseSettings):
    MONGO_URI: str = "mongodb://mongodb:27017"
    MONGO_DB: str = "friendship_db"

settings = Settings()
