# Dockerfile para authService

FROM python:3.10-slim

WORKDIR /app/auth_service

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copiar la app y los scripts
COPY ./app ./app
COPY ./scripts /app/auth_service/scripts

ENV PYTHONPATH=/app/auth_service

EXPOSE 8000

# Ejecuta FastAPI y luego init_admin.py (espera 5s por si RabbitMQ/mongo aún no están listos)
CMD ["sh", "-c", "uvicorn app.main:app --host 0.0.0.0 --port 8000 & sleep 5 && python scripts/init_admin.py && wait"]
