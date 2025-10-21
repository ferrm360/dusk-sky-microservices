from app.database.mongodb import sale_collection
from app.models.sale_model import SaleCreate
from bson import ObjectId

async def create_sale(sale: SaleCreate):
    sale_dict = sale.dict()
    sale_dict["buyers"] = []
    result = await sale_collection.insert_one(sale_dict)
    return str(result.inserted_id)

async def buy_game(sale_id: str, user_id: str) -> bool:
    sale = await sale_collection.find_one({"_id": ObjectId(sale_id)})
    
    if not sale:
        return False  

    if sale["quantity"] <= 0:
        return False  

    if user_id in sale["buyers"]:
        return False  

    await sale_collection.update_one(
        {"_id": ObjectId(sale_id)},
        {
            "$inc": {"quantity": -1},
            "$push": {"buyers": user_id}
        }
    )
    return True

async def get_sales_summary():
    cursor = sale_collection.find()
    result = []
    async for sale in cursor:
        result.append({
            "game_id": sale["game_id"],
            "price": sale["price"],
            "sold": len(sale["buyers"]),
            "available": sale["quantity"]
        })
    return result

async def get_sale_by_game_id(game_id: str):
    sale = await sale_collection.find_one({"game_id": game_id})
    if sale:
        sale["_id"] = str(sale["_id"])
    return sale

async def get_purchases_by_user(user_id: str):
    cursor = sale_collection.find({"buyers": user_id})
    purchases = []
    async for sale in cursor:
        purchases.append({
            "game_id": sale["game_id"],
            "price": sale["price"],
            "purchase_id": str(sale["_id"])
        })
    return purchases
