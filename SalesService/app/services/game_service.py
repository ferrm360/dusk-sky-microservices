# app/services/game_service.py
from motor.motor_asyncio import AsyncIOMotorCollection
from bson.objectid import ObjectId
from typing import List, Optional

from app.models.game import GameCreate, GameInDB, GameUpdate
from app.Database.mongodb import db # Import the global 'db' instance

class GameService:
    def __init__(self):
        if db is None:
            raise RuntimeError("Database connection not initialized. Call connect_to_mongo first.")
        self.collection: AsyncIOMotorCollection = db["games"] 

    async def create_game(self, game_data: GameCreate) -> Optional[GameInDB]:
        """
        Adds a new game to the database.
        Returns the created GameInDB object, or None if a game with the same name already exists.
        """
        existing_game = await self.collection.find_one({"name": game_data.name})
        if existing_game:
            return None 

        game_dict = game_data.dict()
        game_dict["purchasing_users"] = [] 
        result = await self.collection.insert_one(game_dict)
        
        new_game_doc = await self.collection.find_one({"_id": result.inserted_id})
        return GameInDB.parse_obj(new_game_doc) if new_game_doc else None

    async def get_all_games(self) -> List[GameInDB]:
        """
        Retrieves all games from the database.
        Returns a list of GameInDB objects.
        """
        games = []
        cursor = self.collection.find({})
        for game_doc in await cursor.to_list(length=1000): 
            games.append(GameInDB.parse_obj(game_doc))
        return games

    async def get_game_by_id(self, game_id: str) -> Optional[GameInDB]:
        """
        Retrieves a game by its MongoDB ID.
        Returns the GameInDB object if found, or None if not found or ID is invalid.
        """
        try:
            object_id = ObjectId(game_id)
        except Exception:
            return None 

        game_doc = await self.collection.find_one({"_id": object_id})
        return GameInDB.parse_obj(game_doc) if game_doc else None

    async def purchase_game(self, game_id: str, user_id: str) -> Optional[GameInDB]:
      
        try:
            object_id = ObjectId(game_id)
        except Exception:
            return None 


        result = await self.collection.find_one_and_update(
            {"_id": object_id, 
             "available_quantity": {"$gt": 0},
             "purchasing_users": {"$ne": user_id} 
            },
            {
                "$inc": {"available_quantity": -1}, 
                "$push": {"purchasing_users": user_id} 
            },
            return_document=True 
        )
        if result:
            return GameInDB.parse_obj(result)
        else:
           
            return await self.get_game_by_id(game_id)


    async def get_sold_count(self, game_id: str) -> Optional[int]:
        
        game = await self.get_game_by_id(game_id)
        if game:
            return len(game.purchasing_users)
        return None

    async def update_game_stock(self, game_id: str, update_data: GameUpdate) -> Optional[GameInDB]:
        
        try:
            object_id = ObjectId(game_id)
        except Exception:
            return None 

        updated_game_doc = await self.collection.find_one_and_update(
            {"_id": object_id},
            {"$inc": {"available_quantity": update_data.additional_quantity}}, 
            return_document=True 
        )
        return GameInDB.parse_obj(updated_game_doc) if updated_game_doc else None