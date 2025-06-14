from pydantic import BaseModel

class FriendshipRequestDTO(BaseModel):
    sender_id: str
    receiver_id: str