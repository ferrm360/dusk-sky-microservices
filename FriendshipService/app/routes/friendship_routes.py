from uuid import UUID
from fastapi import APIRouter, HTTPException

from app.dto.friendship_dto import FriendshipRequestDTO  # ✅ cambio aquí
from app.controllers import friendship_controller
from app.schemas.friendship_schema import FriendshipRequest

router = APIRouter(prefix="/friendships")


@router.post("/")
async def send_request(dto: FriendshipRequestDTO):
    try:
        sender_uuid = dto.sender_id
        receiver_uuid = dto.receiver_id
    except ValueError:
        raise HTTPException(status_code=400, detail="IDs deben ser UUID válidos")

    return await friendship_controller.send_request(
        str(sender_uuid),
        str(receiver_uuid)
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
