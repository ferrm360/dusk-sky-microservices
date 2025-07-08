db = db.getSiblingDB("GameListService");

db.createUser({
  user: "gamelistUser",
  pwd: "gamelistPassword",
  roles: [
    {
      role: "readWrite",
      db: "GameListService"
    }
  ]
});

