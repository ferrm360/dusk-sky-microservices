import uuid
import datetime 
from sqlalchemy import (
    Column,
    String,
    Enum,     
    DateTime,  
)
from sqlalchemy.schema import UniqueConstraint 
from database import Base 

class Friendship(Base):
    __tablename__ = "Friendship" 

    id = Column(String(36), primary_key=True, default=lambda: str(uuid.uuid4())) 
    sender_id = Column(String(36), nullable=False, index=True) 
    receiver_id = Column(String(36), nullable=False, index=True) 

    status = Column(
        Enum('pending', 'accepted', 'blocked', name='friendship_status_enum'),
        nullable=False,
        default='pending'
    )

    requested_at = Column(DateTime, default=datetime.datetime.utcnow, nullable=True) 

    __table_args__ = (
        UniqueConstraint('sender_id', 'receiver_id', name='_sender_receiver_uc'),
    )

    def __repr__(self):
        return f"<Friendship(id='{self.id}', sender_id='{self.sender_id}', receiver_id='{self.receiver_id}', status='{self.status}', requested_at='{self.requested_at}')>"

from pydantic import BaseModel

class FriendshipRequest(BaseModel):
    sender_id: str
    receiver_id: str



