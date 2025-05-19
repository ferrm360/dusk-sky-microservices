from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy.future import select
from sqlalchemy.orm import sessionmaker
from fastapi import HTTPException
import uuid
from app.Database.database import engine
from app.models.friendship_model import Friendship

async_session = sessionmaker(
    engine,
    class_=AsyncSession,
    expire_on_commit=False,
)

def normalize_ids(user1: str, user2: str) -> tuple[str, str]:
    return tuple(sorted([user1, user2]))

async def send_request(sender_id: str, receiver_id: str):
    user1, user2 = normalize_ids(sender_id, receiver_id)

    async with async_session() as session:
        # Ejecutando la consulta para verificar si ya existe una amistad
        result = await session.execute(
            select(Friendship).filter_by(sender_id=user1, receiver_id=user2)
        )

        # Usa .scalars() directamente para obtener el primer valor de la consulta
        friendship = result.scalars().first()  # No es necesario 'await' aquí

        if friendship:
            print("DEBUG: Friendship exists, raising 400") 
            raise HTTPException(status_code=400, detail="Friendship request already exists")
        
        print("DEBUG: No existing friendship, creating new request") 
        id = str(uuid.uuid4())
        new_friendship = Friendship(id=id, sender_id=sender_id, receiver_id=receiver_id)
        session.add(new_friendship)
        await session.commit()
        print("DEBUG: Request created and committed, returning 200")  # Para la depuración
        return {"id": new_friendship.id, "message": "Friend request sent"}

# Funciones para aceptar o rechazar solicitudes
async def accept_request(request_id: str):
    async with async_session() as session:
        result = await session.execute(
            select(Friendship).filter_by(id=request_id)
        )
        friendship = result.scalars().first()  # No es necesario 'await' aquí

        if not friendship:
            raise HTTPException(status_code=404, detail="Friendship request not found")
        
        friendship.status = "accepted"
        await session.commit()
        return {"message": "Friendship accepted"}

async def reject_request(request_id: str):
    async with async_session() as session:
        result = await session.execute(
            select(Friendship).filter_by(id=request_id)
        )
        friendship = result.scalars().first()  # No es necesario 'await' aquí

        if not friendship:
            raise HTTPException(status_code=404, detail="Friendship request not found")
        
        await session.delete(friendship)
        await session.commit()
        return {"message": "Friend request rejected"}

# Obtener amigos (amistades aceptadas)
async def get_friends(user_id: str):
    async with async_session() as session:
        result = await session.execute(
            select(Friendship).filter(
                Friendship.status == "accepted",
                (Friendship.sender_id == user_id) | (Friendship.receiver_id == user_id)
            )
        )
        friends = result.scalars().all()  # Usar .all() para obtener todos los resultados
        return friends

# Obtener solicitudes pendientes
async def get_pending_requests(user_id: str):
    async with async_session() as session:
        result = await session.execute(
            select(Friendship).filter_by(status="pending", receiver_id=user_id)
        )
        pending_requests = result.scalars().all()  # Usar .all() para obtener todos los resultados
        return pending_requests
