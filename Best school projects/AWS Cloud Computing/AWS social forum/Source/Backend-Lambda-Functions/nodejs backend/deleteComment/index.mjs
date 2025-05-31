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

    const requestBody = event;

    // Extract and validate the required fields
    const { comment_id, user_id } = requestBody;

    if (!comment_id || !user_id) {
      return {
        statusCode: 400,
        body: JSON.stringify({ error: 'Missing required fields: comment_id, user_id' }),
      };
    }


    // Step 1: Check if the comment exists and get its owner
    const [rows] = await connection.execute('SELECT user_id FROM comments WHERE comment_id = ?', [comment_id]);

    if (rows.length === 0) {
      return {
        statusCode: 404,
        body: JSON.stringify({ error: 'Comment not found' }),
      };
    }

    // Get the actual user_id from DB and ensure it matches
    const dbUserId = rows[0].user_id; 

    if (dbUserId !== user_id) {
      return {
        statusCode: 403,
        body: JSON.stringify({ error: 'Unauthorized: You can only delete your own comments' }),
      };
    }

    // Step 2: Perform the delete operation
    const [result] = await connection.execute('DELETE FROM comments WHERE comment_id = ?', [comment_id]);

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Comment deleted successfully' }),
    };
  } catch (error) {
    console.error('Database deletion error:', error);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: 'Failed to delete comment', details: error.message }),
    };
  } finally {
    if (connection) {
      await connection.end();
    }
  }
};
