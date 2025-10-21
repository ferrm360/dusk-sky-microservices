# app/utils/youtube_api_service.py
import requests
import os

YOUTUBE_API_BASE_URL = "https://www.googleapis.com/youtube/v3/search"

def search_videos_by_game(game_name, max_results=8):
    """
    Realiza la llamada a la YouTube Data API v3 para buscar videos.
    """
    api_key = os.getenv("YOUTUBE_API_KEY")
    if not api_key:
        raise ValueError("YOUTUBE_API_KEY no está configurada en el entorno.")

    params = {
        "part": "snippet",
        "q": game_name,
        "type": "video",
        "maxResults": max_results,
        "key": api_key
    }

    response = requests.get(YOUTUBE_API_BASE_URL, params=params)
    response.raise_for_status() # Lanza una excepción para errores 4xx/5xx

    search_data = response.json()
    
    # Limpieza/Mapeo de datos
    videos = []
    for item in search_data.get('items', []):
        snippet = item.get('snippet', {})
        videos.append({
            "title": snippet.get('title'),
            "videoId": item.get('id', {}).get('videoId'),
            "channelTitle": snippet.get('channelTitle'),
            "thumbnailUrl": snippet.get('thumbnails', {}).get('medium', {}).get('url') 
        })
        
    return videos