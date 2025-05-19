from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    MONGODB_URI_AUTH: str
    MONGODB_NAME_AUTH: str = "auth_db"
    RABBITMQ_URL_AUTH: str
    
    JWT_ACCESS_TOKEN_EXPIRE_MINUTES_AUTH: int = 30
    JWT_SECRET_KEY_AUTH: str
    JWT_ALGORITHM_AUTH: str = "HS256"

    class Config:
        env_file = ".env"
        
settings = Settings()
