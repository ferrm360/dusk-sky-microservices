import os
from urllib.parse import urlparse

def delete_previous_files(directory: str, prefix: str):
    for file in os.listdir(directory):
        if file.startswith(prefix):
            full_path = os.path.join(directory, file)
            os.remove(full_path)
            print(f"[INFO] Eliminado: {full_path}")