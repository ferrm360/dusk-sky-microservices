from fastapi import APIRouter, HTTPException, status, Query
from datetime import datetime, timezone
from typing import List

from models.review_model import Review
from controllers import review_controller

router = APIRouter()

@router.post("/")
async def create(review: Review):
    review.createdAt = datetime.now(timezone.utc)
    return await review_controller.create_review(review)

@router.put("/{review_id}/like")
async def like_review_route(review_id: str, user_id: str):
    success = await review_controller.like_review(review_id, user_id)
    if not success:
        raise HTTPException(status_code=404, detail="Like failed")
    return {"liked": True}

@router.put("/{review_id}/unlike")
async def unlike_review_route(review_id: str, user_id: str):
    success = await review_controller.unlike_review(review_id, user_id)
    if not success:
        raise HTTPException(status_code=404, detail="Unlike failed")
    return {"unliked": True}

@router.delete("/{review_id}", status_code=status.HTTP_204_NO_CONTENT)
async def delete(review_id: str, user_id: str):
    if not await review_controller.delete_review(review_id, user_id):
        raise HTTPException(status_code=403, detail="Not authorized or review not found")
    return

@router.get("/recent")
async def get_recent_reviews(limit: int = 10):
    return await review_controller.get_reviews_sorted("createdAt", -1, limit)

@router.get("/top")
async def get_top_reviews(limit: int = 10):
    return await review_controller.get_reviews_sorted("likes", -1, limit)

@router.get("/friends")
async def get_friends_reviews(friend_ids: List[str] = Query(...), limit: int = 10):
    return await review_controller.get_reviews_from_users(friend_ids, limit)

@router.get("/game/{game_id}")
async def get_reviews_by_game(game_id: str):
    return await review_controller.get_reviews_sorted("createdAt", -1, game_id=game_id)

@router.get("/game/{game_id}/recent")
async def get_recent_reviews_by_game(game_id: str, limit: int = 10):
    return await review_controller.get_reviews_sorted("createdAt", -1, limit, game_id)

@router.get("/game/{game_id}/top")
async def get_top_reviews_by_game(game_id: str, limit: int = 10):
    return await review_controller.get_reviews_sorted("likes", -1, limit, game_id)

@router.get("/game/{game_id}/friends")
async def get_friends_reviews_by_game(game_id: str, friend_ids: List[str] = Query(...), limit: int = 10):
    return await review_controller.get_reviews_from_users(friend_ids, limit, game_id)
