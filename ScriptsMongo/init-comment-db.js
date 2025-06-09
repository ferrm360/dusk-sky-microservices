db = db.getSiblingDB("CommentService");

db.createUser({
  user: "commentuser",
  pwd: "commentuserPassword",
  roles: [
    {
      role: "readWrite",
      db: "CommentService"
    }
  ]
});
