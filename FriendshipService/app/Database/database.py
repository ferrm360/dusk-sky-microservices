import os
from dotenv import load_dotenv
from sqlalchemy.ext.asyncio import AsyncSession, create_async_engine
from sqlalchemy.orm import sessionmaker, declarative_base

# Cargar las variables de entorno desde el archivo .env
load_dotenv()

# Obtener las variables del archivo .env
MYSQL_USER = os.getenv("MYSQL_USER")
MYSQL_PASSWORD = os.getenv("MYSQL_PASSWORD")
MYSQL_HOST = os.getenv("MYSQL_HOST")
MYSQL_PORT = os.getenv("MYSQL_PORT")
MYSQL_DB = os.getenv("MYSQL_DB")

# Verificar si las variables están siendo leídas correctamente
print(f"MYSQL_USER: {MYSQL_USER}")
print(f"MYSQL_PASSWORD: {MYSQL_PASSWORD}")
print(f"MYSQL_HOST: {MYSQL_HOST}")
print(f"MYSQL_PORT: {MYSQL_PORT}")
print(f"MYSQL_DB: {MYSQL_DB}")

# Crear la URL de conexión para la base de datos MySQL
DATABASE_URL = f"mysql+aiomysql://{MYSQL_USER}:{MYSQL_PASSWORD}@{MYSQL_HOST}:{MYSQL_PORT}/{MYSQL_DB}"

# Crear el motor de la base de datos
engine = create_async_engine(DATABASE_URL, echo=True)

# Crear la base declarativa
Base = declarative_base()
