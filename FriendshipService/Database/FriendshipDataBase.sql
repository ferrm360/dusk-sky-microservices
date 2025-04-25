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

CREATE UNIQUE INDEX idx_friendship_unique_pair
ON Friendship (
  LEAST(sender_id, receiver_id),
  GREATEST(sender_id, receiver_id)
);
