FROM python:3.10-slim

WORKDIR /app/review_service

# Copia los archivos de dependencias
COPY requirements.txt .

# Instala las dependencias
RUN pip install --no-cache-dir -r requirements.txt

# Copia todo el contenido del proyecto al contenedor
COPY . .

# Expone el puerto que usa FastAPI (por defecto 8000)
EXPOSE 8000

# Comando para correr el servidor con hot-reload (ideal para desarrollo)
CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]
