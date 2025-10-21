# app/controllers/video_controller.py
from flask import jsonify, request
from ..utils.youtube_api_service import search_videos_by_game

def get_related_videos():
    """
    Manejador del endpoint /api/videos/related.
    """
    game_name = request.args.get('game_name')
    
    if not game_name:
        # Petición inválida del cliente
        return jsonify({
            "success": False, 
            "message": "El parámetro 'game_name' es obligatorio."
        }), 400

    try:
        # 1. Llamar a la lógica de negocio/servicio (la API de YouTube)
        videos = search_videos_by_game(game_name)

        # 2. Devolver la respuesta al cliente
        return jsonify({
            "success": True, 
            "videos": videos
        }), 200

    except ValueError as ve:
        # Error de configuración (ej. API Key faltante)
        return jsonify({
            "success": False, 
            "message": "Error de configuración interna."
        }), 500
        
    except requests.exceptions.RequestException as re:
        # Error al comunicarse con la API externa
        print(f"Error de red/API: {re}")
        return jsonify({
            "success": False, 
            "message": "Error al contactar el servicio de videos."
        }), 503 # Servicio no disponible
    except Exception as e:
        # Cualquier otro error inesperado
        print(f"Error inesperado: {e}")
        return jsonify({
            "success": False, 
            "message": "Error interno del servidor."
        }), 500