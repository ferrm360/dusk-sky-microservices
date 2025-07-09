from pydantic import BaseModel, Field
from typing import List
from datetime import datetime, timezone

class Review(BaseModel):
    userId: str = Field(..., json_schema_extra={"example": "uuid-del-usuario"})
    gameId: str = Field(..., json_schema_extra={"example": "steam_appid"})
    content: str
    rating: float = Field(..., ge=0.0, le=5.0)
    createdAt: datetime = Field(default_factory=lambda: datetime.now(timezone.utc))
    likes: int = 0
    likedBy: List[str] = []

class ReviewInDB(Review):
    id: str
