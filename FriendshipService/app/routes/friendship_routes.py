from fastapi import APIRouter
from app.controllers import friendship_controller
from app.schemas.friendship_schema import FriendshipRequest

router = APIRouter(prefix="/friendships")


@router.post("/")
async def send_request(friendship_request: FriendshipRequest):
    return await friendship_controller.send_request(
        str(friendship_request.sender_id),
        str(friendship_request.receiver_id)
    )


@router.put("/{request_id}/accept")
async def accept_request(request_id: str):
    return await friendship_controller.accept_request(request_id)


@router.put("/{request_id}/reject")
async def reject_request(request_id: str):
    return await friendship_controller.reject_request(request_id)


@router.get("/user/{user_id}")
async def get_friends(user_id: str):
    return await friendship_controller.get_friends(user_id)


@router.get("/pending/{user_id}")
async def get_pending_requests(user_id: str):
    return await friendship_controller.get_pending_requests(user_id)
