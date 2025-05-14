# Usa una imagen base de Python ligera
FROM python:3.10-slim

# Crea y usa el directorio de trabajo
WORKDIR /app/user_manager_service

# Copia el archivo de requerimientos primero (para usar la cache si no cambió)
COPY requirements.txt .

# Instala dependencias
RUN pip install --no-cache-dir -r requirements.txt

# Copia el resto de la aplicación (después de requirements.txt por eficiencia de cache)
COPY ./app ./app
COPY .env .

# Expone el puerto que usará el servicio (debe coincidir con docker-compose y FastAPI)
EXPOSE 8003

# Comando para ejecutar el servicio
CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8003", "--reload"]
