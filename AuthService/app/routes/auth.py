# routes/auth.py
from fastapi import APIRouter, Depends, HTTPException, status, Query

from app.models import LoginRequest
from app.models.ChangePasswordRequest import ChangePasswordRequest
from ..models import user, token
from ..controllers import auth_controller
from ..utils import database
from motor.motor_asyncio import AsyncIOMotorDatabase

router = APIRouter()

@router.post("/register/", response_model=user.UserInDB, status_code=status.HTTP_201_CREATED)
async def register_user(user_in: user.UserCreate, db: AsyncIOMotorDatabase = Depends(database.get_database)):
    try:
        new_user = await auth_controller.register_user(user_in, db)
        return new_user
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))

@router.post("/login/", response_model=token.Token)
async def login_for_access_token(login_request: LoginRequest.LoginRequest, db: AsyncIOMotorDatabase = Depends(database.get_database)):
    try:
        token_data = await auth_controller.login_for_access_token(login_request, db)
        return token_data
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail=str(e), headers={"WWW-Authenticate": "Bearer"})
    
@router.delete("/delete/{user_id}")
async def delete_user(user_id: str, db=Depends(database.get_database)):
    try:
        return await auth_controller.delete_user(user_id, db)
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(e))
    

@router.put("/update-username/{user_id}")
async def update_username(user_id: str, new_username: str, db=Depends(database.get_database)):
    try:
        return await auth_controller.update_username(user_id, new_username, db)
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))

@router.put("/update-email/{user_id}")
async def update_email(user_id: str, new_email: str, db=Depends(database.get_database)):
    try:
        return await auth_controller.update_email(user_id, new_email, db)
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))


@router.post("/change-password/{user_id}", status_code=status.HTTP_200_OK)
async def change_password(user_id: str, change_password_request: ChangePasswordRequest, db=Depends(database.get_database)):
    try:
        response = await auth_controller.change_password(
            user_id, 
            change_password_request.current_password, 
            change_password_request.new_password, 
            db
        )
        return response
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))
    
@router.get("/users/search")
async def search_users(username: str = Query(..., min_length=1), db: AsyncIOMotorDatabase = Depends(database.get_database)):
    return await auth_controller.search_users_by_username(username, db)    

@router.get("/users/{user_id}")
async def get_user(user_id: str, db: AsyncIOMotorDatabase = Depends(database.get_database)):
    try:
        return await auth_controller.get_user_by_id(user_id, db)
    except ValueError as e:
        raise HTTPException(status_code=404, detail=str(e))
    

@router.put("/promote/{user_id}", response_model=user.UserInDB)
async def promote_user(user_id: str, db: AsyncIOMotorDatabase = Depends(database.get_database)):
    try:
        updated_user = await auth_controller.promote_user(user_id, db)
        return updated_user
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))

@router.put("/demote/{user_id}", response_model=user.UserInDB)
async def demote_user(user_id: str, db: AsyncIOMotorDatabase = Depends(database.get_database)):
    try:
        updated_user = await auth_controller.demote_user(user_id, db)
        return updated_user
    except ValueError as e:
        raise HTTPException(status_code=status.HTTP_400_BAD_REQUEST, detail=str(e))    
