from pydantic import BaseModel, model_validator
from uuid import UUID
from typing import Optional
from datetime import datetime


class FriendshipRequest(BaseModel):
    sender_id: UUID
    receiver_id: UUID

    @model_validator(mode="after")
    def no_self_friend_request(self) -> "FriendshipRequest":
        if self.sender_id == self.receiver_id:
            raise ValueError("No puedes enviarte solicitud de amistad a ti mismo")
        return self


class FriendshipResponse(BaseModel):
    id: str
    sender_id: UUID
    receiver_id: UUID
    status: str = "pending"
    requested_at: Optional[datetime] = None

    class Config:
        orm_mode = True
