from bson import ObjectId
from pydantic import BaseModel, Field
from datetime import datetime

class UserCreate(BaseModel):
    username: str = Field(max_length=50)
    email: str = Field(max_length=250)
    password: str

class User(BaseModel):
    username: str = Field(max_length=50)
    email: str = Field(max_length=250)
    role: str = Field(default="player")
    status: str = Field(default="active")
    created_at: datetime = Field(default_factory=datetime.utcnow)

class UserInDB(User):
    id: str = Field(alias="_id")

    class Config:
        json_encoders = {
            ObjectId: str
        }

class UserSecret(BaseModel):
    user_id: str
    password_hash: str
    password_salt: str