db = db.getSiblingDB("CommentService");

db.createUser({
  user: "commentUser",
  pwd: "commentUserPassword",
  roles: [
    {
      role: "readWrite",
      db: "CommentService"
    }
  ]
});
