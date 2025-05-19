from fastapi import APIRouter
from app.controllers import friendship_controller

router = APIRouter(prefix="/friendships")

from pydantic import BaseModel

class FriendshipRequest(BaseModel):
    sender_id: str
    receiver_id: str

@router.post("/send")
async def send_request(friendship_request: FriendshipRequest):
    return await friendship_controller.send_request(friendship_request.sender_id, friendship_request.receiver_id)


@router.put("/accept/{request_id}")
async def accept_request(request_id: str):
    return await friendship_controller.accept_request(request_id)

@router.put("/reject/{request_id}")
async def reject_request(request_id: str):
    return await friendship_controller.reject_request(request_id)

@router.get("/friends/{user_id}")
async def get_friends(user_id: str):
    return await friendship_controller.get_friends(user_id)

@router.get("/pending/{user_id}")
async def get_pending_requests(user_id: str):
    return await friendship_controller.get_pending_requests(user_id)


