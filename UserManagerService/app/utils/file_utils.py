import os
from urllib.parse import urlparse

def delete_file_from_url(file_url: str):
    try:
        parsed = urlparse(file_url)
        file_path = parsed.path.replace("/static", "static_user_content")
        if os.path.exists(file_path):
            os.remove(file_path)
            print(f"[INFO] Archivo eliminado: {file_path}")
    except Exception as e:
        print(f"[WARN] No se pudo eliminar archivo anterior: {e}")
