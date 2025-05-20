from pydantic_settings import BaseSettings
from pydantic import Field
from pathlib import Path

class Settings(BaseSettings):
    MONGODB_URI_AUTH: str
    MONGODB_NAME_AUTH: str = "auth_db"
    RABBITMQ_URL_AUTH: str
    
    JWT_ACCESS_TOKEN_EXPIRE_MINUTES_AUTH: int = 30
    JWT_SECRET_KEY_AUTH: str
    JWT_ALGORITHM_AUTH: str = "HS256"

    class Config:
        # Ruta global del archivo .env (ej: desde el root del proyecto)
        env_file = Path(__file__).resolve().parents[2] / ".env"
        env_file_encoding = "utf-8"

    @property
    def database_url(self) -> str:
        return (
            f"mysql+aiomysql://{self.mysql_user}:{self.mysql_password}@"
            f"{self.mysql_host}:{self.mysql_port}/{self.mysql_db}"
        )

# Instancia global
settings = Settings()
