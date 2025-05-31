from functools import wraps
from flask import request, session
from bcrypt import checkpw

from utils.config import config
from utils.db import get_db, release_db
from utils.fields_validator import validate_fields


def handle_login():
    json = request.json
    db, cursor = get_db()

    # Validate fields
    fields = [
        {"field_name": "username", "type": "string", "required": True, "trim": True},
        {"field_name": "password", "type": "string", "required": True}
    ]
    data, errors = validate_fields(fields, json)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    # Get user
    cursor.execute("SELECT * FROM users WHERE username = %s AND role IN ('admin', 'security')", (data["username"],))
    user = cursor.fetchone()
    if not user:
        release_db(db)
        return {"message": "Invalid username or password"}, 401

    # Account locked
    if user["failed_attempts"] > config.get("max_login_attempts", 10):
        release_db(db)
        return {"message": "Your account is locked, please contact your administrator"}, 401

    # Wrong password, increment attempts
    if not checkpw(data["password"].encode(), user["password_hash"].encode()):
        cursor.execute("UPDATE users SET failed_attempts = failed_attempts + 1 WHERE id = %s", (user["id"],))
        db.commit()
        release_db(db)
        return {"message": "Invalid username or password"}, 401

    # Login successful, reset failed attempts
    cursor.execute("UPDATE users SET failed_attempts = 0 WHERE id = %s", (user["id"],))
    db.commit()
    release_db(db)

    # Save login to session
    user_data = {
        "id": user["id"],
        "username": user["username"],
        "role": user["role"]
    }
    session["user"] = user_data

    return {"message": "Login successful", "data": user_data}


def handle_logout():
    session.clear()
    return {"message": "Logout successful"}

def require_roles(roles):
    def decorator(f):
        @wraps(f)
        def decorated_function(*args, **kwargs):
            role = session.get("user", {}).get("role")
            if role not in roles:
                return {"message": "You are not authorised to access this function"}, 403
            return f(*args, **kwargs)
        return decorated_function
    return decorator
