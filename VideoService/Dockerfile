# Dockerfile
# Usamos una imagen base ligera de Python
FROM python:3.11-slim

# Establecemos el directorio de trabajo dentro del contenedor
WORKDIR /usr/src/app

# Copiamos los archivos de requerimientos e instalamos las dependencias
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copiamos todo el código de la aplicación
COPY . .

# Comando para ejecutar la aplicación cuando el contenedor se inicie
CMD ["python", "app.py"]