from pydantic import BaseModel, Field
from typing import List, Optional
from bson import ObjectId

class SaleCreate(BaseModel):
    game_id: str
    price: float
    quantity: int

class Sale(BaseModel):
    id: Optional[str] = Field(alias="_id")
    game_id: str
    price: float
    quantity: int
    buyers: List[str] = []

    class Config:
        arbitrary_types_allowed = True
        json_encoders = {ObjectId: str}
