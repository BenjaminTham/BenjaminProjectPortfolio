import mysql from 'mysql2/promise';

export const handler = async (event) => {
  const dbConfig = {
    host: 'forum-database.ci6qmqse2nc9.us-east-1.rds.amazonaws.com', 
    user: 'admin',
    password: 'testtest',
    database: 'forum-database',
  };

  let connection;

  try {
    connection = await mysql.createConnection(dbConfig);

    // Use event directly as the request body
    const { user_id, topic_id, name, content } = event;

    // Validate required fields
    if (!user_id || !topic_id || !name || !content) {
      return {
        statusCode: 400,
        body: JSON.stringify({ error: "Missing required fields" }),
      };
    }

    // Insert the post into the database
    const query = "INSERT INTO posts (user_id, topic_id, name, content, created_at) VALUES (?, ?, ?, ?, NOW())";
    const values = [user_id, topic_id, name, content];
    const [result] = await connection.execute(query, values);

    return {
      statusCode: 201,
      body: JSON.stringify({
        message: "Post created successfully",
        postId: result.insertId,
      }),
    };
  } catch (error) {
    console.error("Database insertion error:", error);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: "Failed to insert post", details: error.message }),
    };
  } finally {
    if (connection) {
      await connection.end();
    }
  }
};