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
    const { topic_id, idToken } = requestBody;

    if (!topic_id || !idToken) {
      return {
        statusCode: 400,
        body: JSON.stringify({ error: 'Missing required fields: topic_id, idToken'}),
      };
    }


     // Step 1: check if user is admin 
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


    // Step 2: Perform the delete operation
    const [result] = await connection.execute('DELETE FROM topics WHERE topic_id = ?', [topic_id]);

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'topic deleted successfully' }),
    };
  } catch (error) {
    console.error('Database deletion error:', error);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: 'Failed to delete topic', details: error.message }),
    };
  } finally {
    if (connection) {
      await connection.end();
    }
  }
};
