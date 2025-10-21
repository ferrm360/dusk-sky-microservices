# app.py
from flask import Flask
# REMOVER: from dotenv import load_dotenv ❌
import os

# REMOVER: load_dotenv() ❌

# Importa el Blueprint de rutas
from app.routes.video_routes import video_bp

app = Flask(__name__)

app.register_blueprint(video_bp, url_prefix='/api/videos')

if __name__ == '__main__':
    # La aplicación está configurada para el puerto 5000 en el contenedor
    app.run(host='0.0.0.0', debug=True, port=5000)