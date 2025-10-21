# app/routes/video_routes.py
from flask import Blueprint
from ..controllers.video_controller import get_related_videos

# Crea un Blueprint para organizar las rutas
video_bp = Blueprint('videos', __name__)

# Conecta la URL '/related' al controlador
video_bp.route('/related', methods=['GET'])(get_related_videos)