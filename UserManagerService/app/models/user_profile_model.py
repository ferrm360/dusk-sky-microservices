from pydantic import BaseModel, Field, HttpUrl, validator
from typing import List, Optional, Literal
from datetime import datetime
from bson import ObjectId
from app.config.settings import settings


# 🎥 Elemento multimedia del perfil
class MediaItem(BaseModel):
    type: Literal["video", "image", "link"]
    url: HttpUrl


# 🧩 Base común para creación, actualización y DB
class UserProfileBase(BaseModel):
    avatar_url: Optional[HttpUrl] = None
    banner_url: Optional[HttpUrl] = None
    bio: Optional[str] = Field(None, max_length=500)
    about_section: Optional[str] = Field(None, max_length=5000)
    media: List[MediaItem] = Field(default_factory=list)
    favorite_genres: List[str] = Field(default_factory=list)


# 🆕 Modelo para crear perfil
class UserProfileCreate(UserProfileBase):
    user_id: str
    created_at: datetime = Field(default_factory=datetime.utcnow)
    updated_at: datetime = Field(default_factory=datetime.utcnow)

    @validator("avatar_url", pre=True, always=True)
    def default_avatar(cls, v):
        if v is None:
            return f"{settings.STATIC_CONTENT_BASE_URL}/avatars/default_avatar.jpg"
        return v

    @validator("banner_url", pre=True, always=True)
    def default_banner(cls, v):
        if v is None:
            return f"{settings.STATIC_CONTENT_BASE_URL}/banners/default_banner.jpg"
        return v

    class Config:
        populate_by_name = True


# ✏️ Modelo para actualizar perfil
class UserProfileUpdate(BaseModel):
    avatar_url: Optional[HttpUrl] = None
    banner_url: Optional[HttpUrl] = None
    bio: Optional[str] = Field(None, max_length=500)
    about_section: Optional[str] = Field(None, max_length=5000)
    media: Optional[List[MediaItem]] = None
    favorite_genres: Optional[List[str]] = None
    updated_at: datetime = Field(default_factory=datetime.utcnow)

    class Config:
        populate_by_name = True


# 🗃 Modelo de perfil en la base de datos
class UserProfileInDB(UserProfileBase):
    id: str = Field(alias="_id")
    user_id: str
    created_at: datetime
    updated_at: datetime

    @validator("avatar_url", pre=False, always=True)
    def default_avatar_if_missing(cls, v):
        if not v:
            return f"{settings.STATIC_CONTENT_BASE_URL}/avatars/default_avatar.jpg"
        return v

    @validator("banner_url", pre=False, always=True)
    def default_banner_if_missing(cls, v):
        if not v:
            return f"{settings.STATIC_CONTENT_BASE_URL}/banners/default_banner.jpg"
        return v

    class Config:
        from_attributes = True  # reemplaza orm_mode en Pydantic v2
        populate_by_name = True
        json_encoders = {
            ObjectId: str,
            datetime: lambda dt: dt.isoformat()
        }
