from fastapi import APIRouter, HTTPException, status, Depends
from app.models.user_events_models import UsernameUpdateRequest
from app.utils.database import get_database
from motor.motor_asyncio import AsyncIOMotorDatabase
from app.models.user_profile_model import (
    UserProfileCreate,
    UserProfileUpdate,
    UserProfileInDB
)
from app.controllers import user_events_controller, user_profile_controller
from app.utils.file_utils import delete_file_from_url


import os
import uuid
import shutil
from datetime import datetime
from typing import List
from fastapi import UploadFile, File, Form

STATIC_AVATAR_PATH = "static_user_content/avatars"
STATIC_BANNER_PATH = "static_user_content/banners"
STATIC_MEDIA_PATH = "static_user_content/media"

os.makedirs(STATIC_AVATAR_PATH, exist_ok=True)
os.makedirs(STATIC_BANNER_PATH, exist_ok=True)
os.makedirs(STATIC_MEDIA_PATH, exist_ok=True)



router = APIRouter(prefix="/profiles", tags=["User Profiles"])

@router.get("/{user_id}", response_model=UserProfileInDB)
async def get_profile(user_id: str, db: AsyncIOMotorDatabase = Depends(get_database)):
    profile = await user_profile_controller.get_profile_by_user_id(user_id, db)
    if not profile:
        raise HTTPException(status_code=404, detail="Perfil no encontrado")
    return profile

@router.put("/{user_id}", response_model=UserProfileInDB, status_code=status.HTTP_201_CREATED)
async def create_profile(user_id: str, payload: UserProfileCreate, db: AsyncIOMotorDatabase = Depends(get_database)):
    if user_id != payload.user_id:
        raise HTTPException(status_code=400, detail="El ID del usuario en la URL y en el cuerpo no coinciden")

    existing = await user_profile_controller.get_profile_by_user_id(user_id, db)
    if existing:
        raise HTTPException(status_code=409, detail="El perfil ya existe")

    return await user_profile_controller.create_profile(payload, db)

@router.delete("/{user_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_profile(user_id: str, db: AsyncIOMotorDatabase = Depends(get_database)):
    deleted = await user_profile_controller.delete_profile_by_user_id(user_id, db)
    if not deleted:
        raise HTTPException(status_code=404, detail="Perfil no encontrado")
    
@router.patch("/{user_id}/username")
async def change_username(user_id: str, body: UsernameUpdateRequest, db: AsyncIOMotorDatabase = Depends(get_database)):
    new_username = body.new_username

    if not new_username:
        raise HTTPException(status_code=400, detail="Falta 'new_username'")
    
    ok = await user_events_controller.update_username(user_id, new_username, db)
    if not ok:
        raise HTTPException(status_code=404, detail="Usuario no encontrado o sin cambios")
    
    return {"message": "Username actualizado y evento enviado a RabbitMQ"}



@router.patch("/{user_id}/upload", response_model=UserProfileInDB)
async def upload_profile_content(
    user_id: str,
    avatar: UploadFile = File(None),
    banner: UploadFile = File(None),
    media: List[UploadFile] = File(None),
    bio: str = Form(None),
    about_section: str = Form(None),
    db: AsyncIOMotorDatabase = Depends(get_database),
):
    update_data = {}
    static_base_url = os.getenv("STATIC_CONTENT_BASE_URL_USER", "http://localhost:8003/static")

    existing_profile = await user_profile_controller.get_profile_by_user_id(user_id, db)

    if avatar:
        if existing_profile and existing_profile.avatar_url:
            delete_file_from_url(existing_profile.avatar_url)

        filename = f"{user_id}_{uuid.uuid4().hex}_{avatar.filename}"
        path = os.path.join(STATIC_AVATAR_PATH, filename)
        with open(path, "wb") as buffer:
            shutil.copyfileobj(avatar.file, buffer)
        update_data["avatar_url"] = f"{static_base_url}/avatars/{filename}"

    if banner:
        if existing_profile and existing_profile.banner_url:
            delete_file_from_url(existing_profile.banner_url)

        filename = f"{user_id}_{uuid.uuid4().hex}_{banner.filename}"
        path = os.path.join(STATIC_BANNER_PATH, filename)
        with open(path, "wb") as buffer:
            shutil.copyfileobj(banner.file, buffer)
        update_data["banner_url"] = f"{static_base_url}/banners/{filename}"

    if media:
        if existing_profile and existing_profile.media:
            for item in existing_profile.media:
                delete_file_from_url(item["url"])

        media_urls = []
        for file in media:
            filename = f"{user_id}_{uuid.uuid4().hex}_{file.filename}"
            path = os.path.join(STATIC_MEDIA_PATH, filename)
            with open(path, "wb") as buffer:
                shutil.copyfileobj(file.file, buffer)
            url = f"{static_base_url}/media/{filename}"
            media_urls.append({
                "type": "video" if file.content_type.startswith("video") else "image",
                "url": url
            })
        update_data["media"] = media_urls

    if bio:
        update_data["bio"] = bio
    if about_section:
        update_data["about_section"] = about_section

    update_data["updated_at"] = datetime.utcnow()

    updated_profile = await user_profile_controller.update_profile(user_id, UserProfileUpdate(**update_data), db)
    if not updated_profile:
        raise HTTPException(status_code=404, detail="Perfil no encontrado")

    return updated_profile