from pydantic import BaseModel
from uuid import UUID

class FriendshipRequest(BaseModel):
    sender_id: UUID
    receiver_id: UUID
