from psycopg2 import pool
from psycopg2._psycopg import connection, cursor
from psycopg2.extras import RealDictCursor
from dotenv import load_dotenv
from os import getenv

# Load database configuration from environment variables
load_dotenv(".env.local" if getenv("FLASK_ENV") == "development" else ".env")
config = {
    "dbname": getenv("DB_NAME"),
    "user": getenv("DB_USER"),
    "password": getenv("DB_PASSWORD"),
    "host": getenv("DB_HOST"),
    "port": getenv("DB_PORT")
}

# Create connection pool
connection_pool = pool.ThreadedConnectionPool(10, 100, **config)
print("Connected to database")

def get_db() -> tuple[connection, cursor]:  # Specify types for autocomplete
    conn = connection_pool.getconn()
    return conn, conn.cursor(cursor_factory=RealDictCursor)

def release_db(db):
    connection_pool.putconn(db)

