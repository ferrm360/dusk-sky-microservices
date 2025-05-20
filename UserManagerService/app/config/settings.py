# app/config/settings.py
from pydantic_settings import BaseSettings
from pydantic import HttpUrl

class Settings(BaseSettings):
    PROJECT_NAME: str = "UserManagerService"

    MONGODB_URI_USER: str
    MONGODB_NAME_USER: str

    RABBITMQ_URL_USER: str

    STATIC_CONTENT_BASE_URL_USER: HttpUrl = "http://localhost:8003/static"

    class Config:
        env_file = ".env",
        extra = "ignore"  


settings = Settings()
