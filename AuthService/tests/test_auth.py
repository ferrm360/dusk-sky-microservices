import pytest
from fastapi.testclient import TestClient
from app.main import app
from app.utils.database import get_database
from unittest.mock import AsyncMock

client = TestClient(app)

@pytest.fixture(scope="module")
def mock_db(mocker):
    mock_db_instance = AsyncMock()

    mock_users_collection = AsyncMock()
    mock_users_collection.insert_one.return_value.inserted_id = "mocked_id"
    mock_users_collection.find_one.return_value = {
        "_id": "mocked_id",
        "username": "mockeduser",
        "email": "mocked@example.com",
        "password": "newpassword123"
    }

    mock_db_instance.users = mock_users_collection
    mock_db_instance.user_secrets = AsyncMock()

    mocker.patch("app.utils.database.get_database", return_value=mock_db_instance)

    yield mock_db_instance 

    mock_users_collection.delete_many({}) 


@pytest.mark.asyncio
async def test_register_user(mock_db):
    user_data = {
        "username": "mockeduser",
        "email": "mocked@example.com",
        "password": "newpassword123"
    }

    response = client.post("/auth/register/", json=user_data)

    assert response.status_code == 201


@pytest.mark.asyncio
async def test_login_user(mock_db):
    login_data = {
        "email": "mocked@example.com",
        "password": "newpassword123"
    }

    mock_db.users.find_one.return_value = {
        "_id": "mocked_id",
        "username": "mockeduser",
        "email": "mocked@example.com",
        "password": "newpassword123"
    }

    response = client.post("/auth/login/", json=login_data)

    assert response.status_code == 200


@pytest.mark.asyncio
async def test_register_user_validation():
    user_data = {
        "username": "",
        "email": "",
        "password": ""
    }

    response = client.post("/auth/register/", json=user_data)

    assert response.status_code == 400


@pytest.mark.asyncio
async def test_login_invalid_user(mock_db):
    # Datos de login inválidos
    login_data = {
        "email": "wronguser@example.com",
        "password": "wrongpassword"
    }

    mock_db.users.find_one.return_value = None

    response = client.post("/auth/login/", json=login_data)

    assert response.status_code == 401
