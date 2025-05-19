from fastapi import APIRouter, HTTPException, status, Depends
from app.models.user_events_models import UsernameUpdateRequest
from app.utils.database import get_user_manager_database
from motor.motor_asyncio import AsyncIOMotorDatabase
from app.models.user_profile_model import (
    UserProfileCreate,
    UserProfileUpdate,
    UserProfileInDB
)
from app.controllers import user_events_controller, user_profile_controller

router = APIRouter(prefix="/profiles", tags=["User Profiles"])

@router.get("/{user_id}", response_model=UserProfileInDB)
async def get_profile(user_id: str, db: AsyncIOMotorDatabase = Depends(get_user_manager_database)):
    profile = await user_profile_controller.get_profile_by_user_id(user_id, db)
    if not profile:
        raise HTTPException(status_code=404, detail="Perfil no encontrado")
    return profile

@router.put("/{user_id}", response_model=UserProfileInDB, status_code=status.HTTP_201_CREATED)
async def create_profile(user_id: str, payload: UserProfileCreate, db: AsyncIOMotorDatabase = Depends(get_user_manager_database)):
    if user_id != payload.user_id:
        raise HTTPException(status_code=400, detail="El ID del usuario en la URL y en el cuerpo no coinciden")

    existing = await user_profile_controller.get_profile_by_user_id(user_id, db)
    if existing:
        raise HTTPException(status_code=409, detail="El perfil ya existe")

    return await user_profile_controller.create_profile(payload, db)

@router.patch("/{user_id}/username", summary="Change Username")
async def change_username(user_id: str, body: UsernameUpdateRequest, db: AsyncIOMotorDatabase = Depends(get_user_manager_database)):
    ok = await user_events_controller.update_username(user_id, body.new_username, db)
    if not ok:
        raise HTTPException(status_code=500, detail="No se pudo publicar el evento a RabbitMQ")
    return {"message": "Username actualizado y evento enviado a RabbitMQ"}

@router.delete("/{user_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_profile(user_id: str, db: AsyncIOMotorDatabase = Depends(get_user_manager_database)):
    deleted = await user_profile_controller.delete_profile_by_user_id(user_id, db)
    if not deleted:
        raise HTTPException(status_code=404, detail="Perfil no encontrado")
    
@router.patch("/{user_id}/username")
async def change_username(user_id: str,    body: UsernameUpdateRequest, db: AsyncIOMotorDatabase = Depends(get_user_manager_database)):
    new_username = body.new_username

    if not new_username:
        raise HTTPException(status_code=400, detail="Falta 'new_username'")
    
    ok = await user_events_controller.update_username(user_id, new_username, db)
    if not ok:
        raise HTTPException(status_code=404, detail="Usuario no encontrado o sin cambios")
    
    return {"message": "Username actualizado y evento enviado a RabbitMQ"}
