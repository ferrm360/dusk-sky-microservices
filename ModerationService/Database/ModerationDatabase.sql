CREATE DATABASE IF NOT EXISTS ModerationDataBase;
USE ModerationDataBase;

DROP TABLE IF EXISTS Sanction;
DROP TABLE IF EXISTS Report;

CREATE TABLE Report (
  id CHAR(36) PRIMARY KEY,
  reported_user_id CHAR(36) NOT NULL,
  content_type ENUM('comment', 'review', 'profile') NOT NULL,
  reason TEXT,
  reported_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  status ENUM('pending', 'resolved') DEFAULT 'pending'
);

CREATE TABLE Sanction (
  id CHAR(36) PRIMARY KEY,
  report_id CHAR(36) UNIQUE,
  user_id CHAR(36) NOT NULL,
  type ENUM('suspension', 'ban') NOT NULL,
  start_date DATETIME NOT NULL,
  end_date DATETIME,
  reason TEXT,
  FOREIGN KEY (report_id) REFERENCES Report(id)
);
