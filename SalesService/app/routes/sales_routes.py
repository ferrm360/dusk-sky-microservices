from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from app.controllers import sales_controller
from app.models.sale_model import SaleCreate

router = APIRouter()

class BuyRequest(BaseModel):
    user_id: str

@router.post("/")
async def create_sale(sale: SaleCreate):
    sale_id = await sales_controller.create_sale(sale)
    return {"sale_id": sale_id}

@router.post("/{sale_id}/buy")
async def buy_game(sale_id: str, buy_request: BuyRequest):
    success = await sales_controller.buy_game(sale_id, buy_request.user_id)
    if not success:
        raise HTTPException(status_code=400, detail="Out of stock, already purchased, or sale not found")
    return {"message": "Purchase successful"}

@router.get("/summary")
async def sales_summary():
    return await sales_controller.get_sales_summary()

@router.get("/game/{game_id}")
async def get_sale_by_game(game_id: str):
    sale = await sales_controller.get_sale_by_game_id(game_id)
    if not sale:
        raise HTTPException(status_code=404, detail="Game not found in sales")
    return sale

@router.get("/bought/{user_id}")
async def get_user_purchases(user_id: str):
    return await sales_controller.get_purchases_by_user(user_id)
