from flask import request, session

from utils.config import config
from utils.db import get_db, release_db
from utils.fields_validator import validate_fields


def handle_get_users():
    json = request.json
    db, cursor = get_db()

    # Validate fields
    fields = [
        {"field_name": "id", "type": "integer"},
        {"field_name": "username", "type": "string", "trim": True},
        {"field_name": "email", "type": "string", "trim": True},
        {"field_name": "role", "type": "string", "in": ["admin", "security", "personnel"]}
    ]
    data, errors = validate_fields(fields, json)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    cursor.execute(
        f"""
        SELECT id, username, email, role, biometrics_enrolled, alert_subscribed, location,
        CASE WHEN failed_attempts > {config.get("max_login_attempts")} THEN true ELSE false END AS account_locked
        FROM users
        WHERE {' AND '.join([f'{key} = %s' for key in data.keys()]) if data else ' true'}
        ORDER BY id
        """,
        tuple(data.values())
    )
    users = cursor.fetchall()
    release_db(db)

    return {"data": users}

def handle_add_user():
    json = request.json
    db, cursor = get_db()

    # Validate fields
    fields = [
        {"field_name": "username", "type": "string", "required": True, "trim": True, "min": 3, "max": 32, "unique": {"table_name": "users", "column_name": "username"}},
        {"field_name": "email", "type": "string", "trim": True, "required": True, "unique": {"table_name": "users", "column_name": "email"}, "special": ["email"]},
        {"field_name": "password", "type": "string", "required": True, "special": ["password_complexity", "hash_password"], "output_name": "password_hash", "error_message": "Password must be between 8 - 32 characters and contain at least one uppercase, one lowercase, one numeric, and one special character"},
        {"field_name": "role", "type": "string", "required": True, "in": ["admin", "security", "personnel"]},
        {"field_name": "alert_subscribed", "type": "boolean"}
    ]
    data, errors = validate_fields(fields, json, cursor)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    # Personnel cannot subscribe to alerts
    if data["role"] == "personnel":
        data["alert_subscribed"] = False

    # Add user
    cursor.execute(f"INSERT INTO users ({', '.join(data.keys())}) VALUES ({', '.join(['%s' for _ in range(len(data))])})", tuple(data.values()))
    if cursor.rowcount == 0:
        release_db(db)
        return {"message": "Unable to insert user"}, 500

    db.commit()
    release_db(db)

    return {"message": "User added successfully"}

def handle_update_user(user_id):
    json = request.json
    db, cursor = get_db()

    # Get user
    cursor.execute("SELECT * FROM users WHERE id = %s", (user_id,))
    user = cursor.fetchone()

    # User does not exist
    if not user:
        release_db(db)
        return {"message": "User does not exist"}, 400

    # Validate fields
    fields = [
        {"field_name": "username", "type": "string", "trim": True, "unique": {"table_name": "users", "column_name": "username", "id": user_id}},
        {"field_name": "email", "type": "string", "trim": True, "unique": {"table_name": "users", "column_name": "email", "id": user_id}, "special": ["email"]},
        {"field_name": "password", "type": "string", "special": ["password_complexity", "hash_password"], "output_name": "password_hash", "error_message": "Password must be between 8 - 32 characters and contain at least one uppercase, one lowercase, one numeric, and one special character"},
        {"field_name": "role", "type": "string", "in": ["admin", "security", "personnel"]},
        {"field_name": "alert_subscribed", "type": "boolean"},
        {"field_name": "unlock_account", "type": "boolean", "output_name": "failed_attempts"}
    ]
    data, errors = validate_fields(fields, json, cursor)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    # Personnel cannot subscribe to alerts (based on updated role else current role)
    if data.get("role", user.get("role")) == "personnel":
        data["alert_subscribed"] = False

    # Unlock account request
    if data.get("failed_attempts"):
        data["failed_attempts"] = 0

    # If biometrics are already enrolled, then cannot change username
    if user.get("biometrics_enrolled") and data.get("username") and data.get("username") != user.get("username"):
        release_db(db)
        return {"message": "Cannot change username as biometrics already enrolled"}, 400

    # Update user
    cursor.execute(f"UPDATE users SET {', '.join([f'{_} = %s' for _ in data.keys()])} WHERE id = %s", tuple(list(data.values()) + [user_id]))
    if cursor.rowcount == 0:
        release_db(db)
        return {"message": "Unable to update user"}, 500

    db.commit()
    release_db(db)

    return {"message": "User updated successfully"}

def handle_delete_user(user_id):
    # Cannot delete own account
    user_id = int(user_id)
    current_user_id = session.get("user").get("id")
    if current_user_id == user_id:
        return {"message": "Cannot delete your own account"}, 400

    db, cursor = get_db()

    # Get user
    cursor.execute("SELECT * FROM users WHERE id = %s", (user_id,))
    user = cursor.fetchone()

    # User does not exist
    if not user:
        release_db(db)
        return {"message": "User does not exist"}, 400

    # Cannot delete user with biometrics enrolled
    if user.get("biometrics_enrolled"):
        release_db(db)
        return {"message": "Unable to remove users with biometrics enrolled, please delete biometrics first"}

    # Delete user
    cursor.execute("DELETE FROM users WHERE id = %s", (user_id,))
    if cursor.rowcount == 0:
        release_db(db)
        return {"message": "Unable to delete user"}, 500

    db.commit()
    release_db(db)
    return {"message": "User deleted successfully"}
