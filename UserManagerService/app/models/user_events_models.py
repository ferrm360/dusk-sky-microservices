from pydantic import BaseModel

class UsernameUpdateRequest(BaseModel):
    new_username: str
