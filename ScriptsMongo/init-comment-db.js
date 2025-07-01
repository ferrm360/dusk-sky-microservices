// ScriptsMongo/init-comment-db.js

db = db.getSiblingDB("CommentService");

db.createUser({
  user: "commentuser",  // debe coincidir con COMMENT_MONGO_USER
  pwd: "commentuserPassword",  // debe coincidir con COMMENT_MONGO_PASSWORD
  roles: [
    {
      role: "readWrite",
      db: "CommentService" // debe coincidir con COMMENT_MONGO_DATABASE
    }
  ]
});
