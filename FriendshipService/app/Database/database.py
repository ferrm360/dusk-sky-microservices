from sqlalchemy.ext.asyncio import AsyncSession, create_async_engine
from sqlalchemy.orm import sessionmaker, declarative_base
from app.config.settings import settings

# Crear el motor de la base de datos con la URL construida desde las variables de entorno
engine = create_async_engine(settings.database_url, echo=True)

# Crear la base declarativa (para tus modelos ORM)
Base = declarative_base()

# Crear sesión asíncrona reutilizable
async_session = sessionmaker(
    bind=engine,
    class_=AsyncSession,
    expire_on_commit=False
)
