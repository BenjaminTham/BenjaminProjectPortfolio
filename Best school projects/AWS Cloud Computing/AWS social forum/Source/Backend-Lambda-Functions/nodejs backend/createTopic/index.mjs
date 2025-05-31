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
    // Parse the JSON request body
    // const requestBody = JSON.parse(event.body || '{}');
    const requestBody = event;

    // Extract name and description from request
    const { name, description, idToken } = requestBody;

    // Validate input
    if (!name || !description || !idToken) {
      return {
        statusCode: 400,
        body: JSON.stringify({ error: 'Missing required fields: name , description and idToken' }),
      };
    }

    connection = await mysql.createConnection(dbConfig);

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


    // Else : Insert a new topic
    const insertQuery = `INSERT INTO topics (name, description) VALUES (?, ?)`;
    const values = [name, description];

    const [result] = await connection.execute(insertQuery, values);

    return {
      statusCode: 200,
      body: JSON.stringify({ message: 'Topic inserted successfully!', insertId: result.insertId }),
    };
  } catch (error) {
    console.error('Database connection error:', error);
    return {
      statusCode: 500,
      body: JSON.stringify({ error: 'Database connection failed' }),
    };
  } finally {
    if (connection) {
      await connection.end();
    }
  }
};
