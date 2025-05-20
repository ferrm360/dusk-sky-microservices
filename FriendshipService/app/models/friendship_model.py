from pydantic import BaseModel, Field
from typing import Literal, Optional
from datetime import datetime

class FriendshipBase(BaseModel):
    sender_id: str
    receiver_id: str

class FriendshipCreate(FriendshipBase):
    pass  # útil si luego quieres validar más cosas al crear

class FriendshipResponse(FriendshipBase):
    id: str = Field(..., alias="_id")
    status: Literal["pending", "accepted", "blocked"]
    requested_at: Optional[datetime]

    class Config:
        allow_population_by_field_name = True  # para usar _id como id
        orm_mode = True
