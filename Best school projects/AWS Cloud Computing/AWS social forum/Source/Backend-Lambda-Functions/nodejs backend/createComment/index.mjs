import mysql from 'mysql2/promise';

export const handler = async (event) => {
    console.log('Event Body:', event.body);

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
      const { user_id, post_id, content } = event;
  
      // Validate required fields
      if (!user_id || !post_id || !content) {
        return {
          statusCode: 400,
          body: JSON.stringify({ error: 'Missing required fields: user_id, post_id, content' }),
        };
      }
  
      // Insert query
      const query = 'INSERT INTO comments (user_id, post_id, content, created_at) VALUES (?, ?, ?, NOW())';
      const values = [user_id, post_id, content];
      const [result] = await connection.execute(query, values);
  
      return {
        statusCode: 201,
        body: JSON.stringify({ message: 'Comment added successfully', commentId: result.insertId }),
      };
    } catch (error) {
      console.error('Database insertion error:', error);
      return {
        statusCode: 500,
        body: JSON.stringify({ error: 'Failed to insert comment', details: error.message }),
      };
    } finally {
      if (connection) {
        await connection.end();
      }
    }
  };
  