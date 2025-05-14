# app/config/settings.py
from pydantic_settings import BaseSettings
from pydantic import HttpUrl

class Settings(BaseSettings):
    PROJECT_NAME: str = "UserManagerService"

    MONGODB_URI: str
    MONGODB_NAME: str

    RABBITMQ_URL: str

    STATIC_CONTENT_BASE_URL: HttpUrl = "http://localhost:8003/static"

    class Config:
        env_file = ".env",
        extra = "ignore"  


settings = Settings()
