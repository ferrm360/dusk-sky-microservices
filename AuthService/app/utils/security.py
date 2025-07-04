import bcrypt
from fastapi import Depends, HTTPException
from fastapi.security import OAuth2PasswordBearer
from passlib.context import CryptContext
from datetime import datetime, timedelta
import jwt
from ..config import settings
from typing import Any


pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

def hash_password(password: str) -> tuple[bytes, bytes]:
    salt = bcrypt.gensalt()
    hashed_password = bcrypt.hashpw(password.encode('utf-8'), salt)
    return hashed_password, salt

def verify_password(plain_password: str, hashed_password: str) -> bool:
    return bcrypt.checkpw(plain_password.encode('utf-8'), hashed_password)

def create_access_token(subject: str, user_data: dict[str, Any], expires_delta: timedelta = None) -> str:
    if expires_delta:
        expire = datetime.utcnow() + expires_delta
    else:
        expire = datetime.utcnow() + timedelta(minutes=settings.JWT_ACCESS_TOKEN_EXPIRE_MINUTES_AUTH)
    to_encode = {"exp": expire, "sub": str(subject)}
    to_encode.update(user_data)

    encoded_jwt = jwt.encode(to_encode, settings.settings.JWT_SECRET_KEY_AUTH, algorithm=settings.settings.JWT_ALGORITHM_AUTH)
    return encoded_jwt

def decode_access_token(token: str) -> str | None:
    try:
        payload = jwt.decode(token, settings.settings.JWT_SECRET_KEY, algorithms=[settings.settings.JWT_ALGORITHM_AUTH])
        return payload.get("sub")
    except jwt.PyJWTError:
        return None
    
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="auth/login")

def verify_token(token: str = Depends(oauth2_scheme)) -> dict:
    try:
        payload = jwt.decode(token, settings.settings.JWT_SECRET_KEY, algorithms=[settings.settings.JWT_ALGORITHM_AUTH])
        return payload  
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.JWTError:
        raise HTTPException(status_code=401, detail="Invalid token")

    return payload