CREATE DATABASE IF NOT EXISTS FriendshipDataBase;
USE FriendshipDataBase;

DROP TABLE IF EXISTS Friendship;

CREATE TABLE Friendship (
  id CHAR(36) PRIMARY KEY,
  sender_id CHAR(36) NOT NULL,
  receiver_id CHAR(36) NOT NULL,
  status ENUM('pending', 'accepted', 'blocked') NOT NULL DEFAULT 'pending',
  requested_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Índice único solo por orden directa, sin usar funciones
CREATE UNIQUE INDEX idx_sender_receiver_unique
ON Friendship (sender_id, receiver_id);
