import sys
import os
import pytest
from unittest.mock import AsyncMock, patch, MagicMock
from fastapi.testclient import TestClient
from datetime import datetime, timezone

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))
from main import app

client = TestClient(app)

@pytest.fixture
def sample_review():
    return {
        "userId": "test-user",
        "gameId": "test-game",
        "content": "Muy buen juego",
        "rating": 4.5,
        "createdAt": datetime.now(timezone.utc).isoformat(),
        "likes": 0,
        "likedBy": []
    }

# Crear reseña
@patch("controllers.review_controller.collection.insert_one", new_callable=AsyncMock)
def test_create_review(mock_insert, sample_review):
    mock_insert.return_value.inserted_id = "mocked-id"
    res = client.post("/reviews/", json=sample_review)
    assert res.status_code == 200
    assert res.json()["id"] == "mocked-id"

# Eliminar reseña
@patch("controllers.review_controller.collection.delete_one", new_callable=AsyncMock)
def test_delete_review(mock_delete):
    mock_delete.return_value.deleted_count = 1
    res = client.delete("/reviews/507f1f77bcf86cd799439011?user_id=test-user")
    assert res.status_code == 204

# Dar like
@patch("controllers.review_controller.collection.update_one", new_callable=AsyncMock)
def test_like_review(mock_update):
    mock_update.return_value.modified_count = 1
    res = client.put("/reviews/507f1f77bcf86cd799439011/like?user_id=test-user")
    assert res.status_code == 200
    assert res.json()["liked"] is True

# Quitar like
@patch("controllers.review_controller.collection.update_one", new_callable=AsyncMock)
def test_unlike_review(mock_update):
    mock_update.return_value.modified_count = 1
    res = client.put("/reviews/507f1f77bcf86cd799439011/unlike?user_id=test-user")
    assert res.status_code == 200
    assert res.json()["unliked"] is True

# Utilidad para construir un cursor simulado con sort + limit + async iteration
def build_mock_cursor(data):
    mock_cursor = MagicMock()
    mock_cursor.sort.return_value.limit.return_value.__aiter__.return_value = data
    return mock_cursor

# Obtener reseñas recientes
def test_get_recent_reviews():
    data = [{
        "_id": "1", "userId": "u", "gameId": "g", "content": "c",
        "rating": 5.0, "createdAt": "now", "likes": 0, "likedBy": []
    }]
    mock_cursor = build_mock_cursor(data)

    with patch("controllers.review_controller.collection.find", return_value=mock_cursor):
        res = client.get("/reviews/recent")
        assert res.status_code == 200
        assert res.json()[0]["userId"] == "u"

# Obtener reseñas top
def test_get_top_reviews():
    data = [{
        "_id": "2", "userId": "u", "gameId": "g", "content": "c",
        "rating": 4.0, "createdAt": "now", "likes": 10, "likedBy": []
    }]
    mock_cursor = build_mock_cursor(data)

    with patch("controllers.review_controller.collection.find", return_value=mock_cursor):
        res = client.get("/reviews/top")
        assert res.status_code == 200
        assert res.json()[0]["likes"] == 10

# Obtener reseñas de amigos
def test_get_friends_reviews():
    data = [{
        "_id": "3", "userId": "friend1", "gameId": "g", "content": "amigo",
        "rating": 3.5, "createdAt": "now", "likes": 2, "likedBy": []
    }]
    mock_cursor = build_mock_cursor(data)

    with patch("controllers.review_controller.collection.find", return_value=mock_cursor):
        res = client.get("/reviews/friends?friend_ids=friend1&friend_ids=friend2")
        assert res.status_code == 200
        assert len(res.json()) == 1


# Obtener reseñas recientes de un juego
def test_get_recent_reviews_by_game():
    data = [{
        "_id": "4", "userId": "u", "gameId": "g123", "content": "reseña 1",
        "rating": 4.8, "createdAt": "now", "likes": 1, "likedBy": []
    }]
    mock_cursor = build_mock_cursor(data)

    with patch("controllers.review_controller.collection.find", return_value=mock_cursor):
        res = client.get("/reviews/game/g123/recent")
        assert res.status_code == 200
        assert res.json()[0]["gameId"] == "g123"

# Obtener reseñas top de un juego
def test_get_top_reviews_by_game():
    data = [{
        "_id": "5", "userId": "u", "gameId": "g123", "content": "reseña top",
        "rating": 4.9, "createdAt": "now", "likes": 20, "likedBy": []
    }]
    mock_cursor = build_mock_cursor(data)

    with patch("controllers.review_controller.collection.find", return_value=mock_cursor):
        res = client.get("/reviews/game/g123/top")
        assert res.status_code == 200
        assert res.json()[0]["likes"] == 20

# Obtener todas las reseñas de un juego
def test_get_all_reviews_by_game():
    data = [{
        "_id": "6", "userId": "u", "gameId": "g456", "content": "todo bien",
        "rating": 4.0, "createdAt": "now", "likes": 5, "likedBy": []
    }]
    mock_cursor = build_mock_cursor(data)

    with patch("controllers.review_controller.collection.find", return_value=mock_cursor):
        res = client.get("/reviews/game/g456")
        assert res.status_code == 200
        assert res.json()[0]["gameId"] == "g456"

# Obtener reseñas de amigos de un juego específico
def test_get_friends_reviews_by_game():
    data = [{
        "_id": "7", "userId": "friend2", "gameId": "g789", "content": "juegazo",
        "rating": 5.0, "createdAt": "now", "likes": 7, "likedBy": []
    }]
    mock_cursor = build_mock_cursor(data)

    with patch("controllers.review_controller.collection.find", return_value=mock_cursor):
        res = client.get("/reviews/game/g789/friends?friend_ids=friend1&friend_ids=friend2")
        assert res.status_code == 200
        assert res.json()[0]["userId"] == "friend2"

