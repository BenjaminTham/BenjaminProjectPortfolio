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

    // Fetch posts, and JOIN usernames from users table
    let query = `
      SELECT posts.*, users.username
      FROM posts
      JOIN users ON posts.user_id = users.user_id
    `;
    let values = [];

    // Parse event body to get POST data
    if (event.body) {
      const body = JSON.parse(event.body);
      
      if (body.post_id) {
        query += ' WHERE topic_id = ?'; 
        values.push(body.topic_id);
      }
    }

    const [rows] = await connection.execute(query, values);

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Query successful', data: rows, event: event }),
    };
  } catch (error) {
    console.error('Database query error:', error);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: 'Database query failed' }),
    };
  } finally {
    if (connection) {
      await connection.end();
    }
  }
};
