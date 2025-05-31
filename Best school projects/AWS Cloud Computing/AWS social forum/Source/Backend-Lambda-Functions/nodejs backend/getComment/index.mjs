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

    let query = `
      SELECT comments.*, users.username
      FROM comments
      JOIN users ON comments.user_id = users.user_id
    `;
    let values = [];

    // Parse event body to get post_id
    if (event.body) {
      try {
        const body = JSON.parse(event.body);
        
        if (body.post_id) {
          query += ' WHERE post_id = ?';
          values.push(body.post_id);
        }
      } catch (error) {
        return {
          statusCode: 400,
          body: JSON.stringify({ error: 'Invalid JSON format' }),
        };
      }
    }

    const [rows] = await connection.execute(query, values);

    if (rows.length === 0) {
      return {
        statusCode: 200,
        body: JSON.stringify({ message: 'No data found' }),
      };
    }

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Query successful', comments: rows }),
    };
  } catch (error) {
    console.error('Database query error:', error);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: 'Database query failed', details: error.message }),
    };
  } finally {
    if (connection) {
      await connection.end();
    }
  }
};
    