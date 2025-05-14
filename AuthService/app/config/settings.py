from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    MONGODB_URI: str
    MONGODB_NAME: str = "auth_db"
    RABBITMQ_URL: str # <--- ¡AÑADE ESTA LÍNEA!

    
    JWT_ACCESS_TOKEN_EXPIRE_MINUTES: int = 30
    JWT_SECRET_KEY: str
    JWT_ALGORITHM: str = "HS256"

    class Config:
        env_file = ".env"
        
settings = Settings()
