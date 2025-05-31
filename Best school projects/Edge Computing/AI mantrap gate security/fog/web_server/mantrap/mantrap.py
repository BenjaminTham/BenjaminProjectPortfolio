import json
import uuid
from json import dumps, JSONDecodeError
from re import match

from utils.db import get_db, release_db
from utils.fields_validator import validate_fields
from flask import request
from uuid import uuid4


def handle_get_mantraps():
    json = request.json
    db, cursor = get_db()

    # Validate fields
    fields = [
        {"field_name": "id", "type": "integer"},
        {"field_name": "location", "type": "string"}
    ]
    data, errors = validate_fields(fields, json, cursor)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    cursor.execute(
        f"""
        SELECT id, location, token, entry_gate_status, exit_gate_status FROM mantraps
        WHERE {' AND '.join([f'{key} = %s' for key in data.keys()]) if data else ' true'}
        ORDER BY id
        """, tuple(data.values()))
    mantraps = cursor.fetchall()
    release_db(db)

    return {"data": mantraps}


def handle_add_mantrap():
    json = request.json
    db, cursor = get_db()

    # Validate fields
    fields = [
        {"field_name": "location", "type": "string", "required": True, "min": 3, "max": 32,
         "unique": {"table_name": "mantraps", "column_name": "location"}}
    ]
    data, errors = validate_fields(fields, json, cursor)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    # Generate token
    data["token"] = uuid4().hex

    # Add mantrap
    cursor.execute("INSERT INTO mantraps (location, token) VALUES (%s, %s)", tuple(data.values()))
    if cursor.rowcount == 0:
        release_db(db)
        return {"message": "Unable to add mantrap"}, 500

    db.commit()
    release_db(db)

    return {"message": "Mantrap added successfully"}


def handle_update_mantrap(mantrap_id):
    json = request.json
    db, cursor = get_db()

    # Validate fields
    fields = [
        {"field_name": "location", "type": "string", "required": True, "min": 3, "max": 32,
         "unique": {"table_name": "mantraps", "column_name": "location", "id": mantrap_id}}
    ]
    data, errors = validate_fields(fields, json, cursor)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    # Update mantrap
    cursor.execute("UPDATE mantraps SET location = %s WHERE id = %s", tuple(list(data.values()) + [mantrap_id]))
    if cursor.rowcount == 0:
        release_db(db)
        return {"message": "Unable to update mantrap"}, 400

    db.commit()
    release_db(db)
    return {"message": "Mantrap updated successfully"}


def handle_delete_mantrap(mantrap_id):
    db, cursor = get_db()

    # Delete mantrap
    cursor.execute("DELETE FROM mantraps WHERE id = %s", (mantrap_id,))
    if cursor.rowcount == 0:
        release_db(db)
        return {"message": "Mantrap does not exist"}, 400

    db.commit()
    release_db(db)
    return {"message": "Mantrap deleted successfully"}


def handle_command_door(mantrap_id, action):
    db, cursor = get_db()

    # Get mantrap
    cursor.execute("SELECT * FROM mantraps WHERE id = %s", (mantrap_id,))
    if cursor.rowcount == 0:
        release_db(db)
        return {"message": "Mantrap does not exist"}, 400

    # Invalid command
    if action not in ("open", "close"):
        release_db(db)
        return {"message": "Invalid action"}, 400

    # Only one mantrap for now
    from utils.mqtt import publish_mqtt
    publish_mqtt("command", dumps({"command": action}))

    return {"message": f"Command sent to {action} mantrap"}


def handle_gate_status(topic, payload):
    db, cursor = get_db()

    # Parse topic
    gate = "entry_gate" if topic.split("/")[0] == "gate_1" else "exit_gate"

    # Parse payload
    try:
        action, timestamp = list(json.loads(payload).items())[0]
        if action not in ("opened", "closed") or not match("^\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}$", timestamp):
            raise ValueError
    except (IndexError, JSONDecodeError, ValueError):
        print(f"[MQTT] Received invalid payload on {topic}")
        release_db(db)
        return

    action = "OPENED" if action == "opened" else "CLOSED"
    print(f"[MQTT] Received {action} on {topic} at {timestamp}")
    # Update database
    cursor.execute(f"UPDATE mantraps SET {gate}_status = %s", (action,))

    category = "door_movement"
    message = f"{gate} {action}".lower().replace("_", " ")

    # Log event
    cursor.execute(f"INSERT INTO logs (category, message, timestamp) VALUES (%s, %s, %s)", (category, message, timestamp))
    db.commit()
    release_db(db)

