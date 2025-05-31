import mysql from 'mysql2/promise';

export const handler = async (event) => {
  const dbConfig = {
    host: 'forum-database.ci6qmqse2nc9.us-east-1.rds.amazonaws.com', // Replace with your RDS endpoint
    user: 'admin',
    password: 'testtest',
    database: 'forum-database',
  };

  let connection;

  try {
    connection = await mysql.createConnection(dbConfig);

    const requestBody = event;

    // Extract required fields
    const { comment_id, user_id, content } = requestBody;

    if (!comment_id || !user_id || !content) {
      return {
        statusCode: 400,
        body: JSON.stringify({ error: 'Missing required fields: comment_id, user_id, content' }),
      };
    }

    // Step 1: Check if the comment exists and belongs to the user
    const [rows] = await connection.execute('SELECT user_id FROM comments WHERE comment_id = ?', [comment_id]);

    if (rows.length === 0) {
      return {
        statusCode: 404,
        body: JSON.stringify({ error: 'Comment not found' }),
      };
    }

    if (rows[0].user_id !== user_id) {
      return {
        statusCode: 403,
        body: JSON.stringify({ error: 'Unauthorized: You can only edit your own comments' }),
      };
    }

    // Step 2: Perform the update
    const [result] = await connection.execute(
      'UPDATE comments SET content = ? WHERE comment_id = ?',
      [content, comment_id]
    );

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Comment updated successfully' }),
    };
  } catch (error) {
    console.error('Database update error:', error);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: 'Failed to update comment', details: error.message }),
    };
  } finally {
    if (connection) {
      await connection.end();
    }
  }
};
