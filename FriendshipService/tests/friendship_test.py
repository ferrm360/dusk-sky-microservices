# test/friendship_test.py
import sys
import os
import pytest
from unittest.mock import AsyncMock, patch, MagicMock
from fastapi.testclient import TestClient
from uuid import uuid4

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from app.main import app

@pytest.fixture
def client():
    with TestClient(app) as client:
        yield client

@pytest.fixture
def sample_request():
    return {
        "sender_id": str(uuid4()),
        "receiver_id": str(uuid4()),
    }

@pytest.fixture
def mock_async_session():
    with patch('app.controllers.friendship_controller.async_session', new_callable=MagicMock) as mock_sessionmaker:
        mock_session = AsyncMock()

        mock_sessionmaker.return_value.__aenter__.return_value = mock_session
        mock_sessionmaker.return_value.__aexit__.return_value = None 

        yield mock_session


def test_accept_request(client):
    request_id = str(uuid4())
    with patch("app.controllers.friendship_controller.accept_request", new_callable=AsyncMock) as mock_accept:
        mock_accept.return_value = {"message": "Friendship accepted"}
        response = client.put(f"/friendships/accept/{request_id}")
        assert response.status_code == 200
        assert response.json()["message"] == "Friendship accepted"

def test_reject_request(client):
    request_id = str(uuid4())
    with patch("app.controllers.friendship_controller.reject_request", new_callable=AsyncMock) as mock_reject:
        mock_reject.return_value = {"message": "Friend request rejected"}
        response = client.put(f"/friendships/reject/{request_id}")
        assert response.status_code == 200
        assert response.json()["message"] == "Friend request rejected"

def test_get_friends(client):
    user_id = str(uuid4())
    with patch("app.controllers.friendship_controller.get_friends", new_callable=AsyncMock) as mock_get:
        mock_get.return_value = [{"id": str(uuid4()), "friend_id": str(uuid4()), "status": "accepted"}]
        response = client.get(f"/friendships/friends/{user_id}")
        assert response.status_code == 200
        assert isinstance(response.json(), list)

def test_get_pending_requests(client):
    user_id = str(uuid4())
    with patch("app.controllers.friendship_controller.get_pending_requests", new_callable=AsyncMock) as mock_get:
        mock_get.return_value = [{"id": str(uuid4()), "sender_id": str(uuid4()), "status": "pending"}]
        response = client.get(f"/friendships/pending/{user_id}")
        assert response.status_code == 200
        assert isinstance(response.json(), list)



def test_send_request_success(client, mock_async_session):
    sender_id = str(uuid4())
    receiver_id = str(uuid4())
    expected_uuid = str(uuid4())

    with patch("app.controllers.friendship_controller.uuid.uuid4", return_value=expected_uuid):

        mock_result = MagicMock()
        mock_scalars = MagicMock()
        mock_scalars.first.return_value = None
        mock_result.scalars.return_value = mock_scalars
        mock_async_session.execute.return_value = mock_result

        response = client.post(
            "/friendships/send",
            json={"sender_id": sender_id, "receiver_id": receiver_id}
        )

        assert response.status_code == 200
        assert response.json() == {"id": expected_uuid, "message": "Friend request sent"}



def test_send_request_already_exists(client, mock_async_session):
    sender_id = str(uuid4())
    receiver_id = str(uuid4())

    mock_result = MagicMock()
    mock_scalars = MagicMock()
    mock_existing_friendship = MagicMock()
    mock_scalars.first.return_value = mock_existing_friendship
    mock_result.scalars.return_value = mock_scalars
    mock_async_session.execute.return_value = mock_result

    response = client.post(
        "/friendships/send",
        json={"sender_id": sender_id, "receiver_id": receiver_id}
    )

    assert response.status_code == 400
    assert response.json().get("detail") == "Friendship request already exists"


