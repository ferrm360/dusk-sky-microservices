from pydantic_settings import BaseSettings
from pydantic import Field


class Settings(BaseSettings):
    mysql_user: str = Field(..., env="MYSQL_USER")
    mysql_password: str = Field(..., env="MYSQL_PASSWORD")
    mysql_host: str = Field(..., env="MYSQL_HOST")
    mysql_port: int = Field(..., env="MYSQL_PORT")
    mysql_db: str = Field(..., env="MYSQL_DB")

    class Config:
        env_file = ".env"
        env_file_encoding = "utf-8"

    @property
    def database_url(self) -> str:
        return (
            f"mysql+aiomysql://{self.mysql_user}:{self.mysql_password}@"
            f"{self.mysql_host}:{self.mysql_port}/{self.mysql_db}"
        )

# Instancia global
settings = Settings()
