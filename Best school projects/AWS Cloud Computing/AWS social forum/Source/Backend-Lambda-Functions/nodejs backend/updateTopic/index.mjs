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
    const { topic_id, name, description, idToken } = requestBody;

    if (!topic_id || !name || !description || !idToken) {
      return {
        statusCode: 400,
        body: JSON.stringify({ error: 'Missing required fields: topic_id, name, description, idToken' }),
      };
    }

    // // Step 1: Retrieve topic_id and check if user is admin
    // const [rows] = await connection.execute('SELECT admin FROM users WHERE user_id = ?', [user_id]);


    // if (rows[0].admin !== 1) {
    //   return {
    //     statusCode: 403,
    //     body: JSON.stringify({ error: 'Unauthorized: Only Administrators can update topics' }),
    //   };
    // }
    const token = idToken;
    const tokenPayload = JSON.parse(atob(token.split(".")[1]));
    const isUserAdmin = tokenPayload["cognito:groups"]?.includes("forum-admin") || false;


    // if not admin, return unauthorised
    if (!isUserAdmin) {
         return {
        statusCode: 403,
        body: JSON.stringify({ error: 'Unauthorized: Only Administrators can update topics' }),
      };
    }

    // Step 2: if admin, proceed to update
    const [result] = await connection.execute(
      'UPDATE topics SET name = ?, description = ? WHERE topic_id = ?;',
      [name, description, topic_id]
    );

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'topic updated successfully' }),
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
