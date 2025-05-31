import binascii
from base64 import b64decode
from json import loads
from os import makedirs
from os.path import join
from pathlib import Path
from re import match

from utils.db import release_db, get_db
from utils.email_sender import send_email
from utils.fields_validator import validate_fields
from flask import request, send_file
from datetime import datetime

makedirs("log/alerts", exist_ok=True)

def handle_get_logs():
    json = request.json
    db, cursor = get_db()

    # Validate fields
    fields = [
        {"field_name": "category", "type": "string", "trim": True},
        {"field_name": "user_id", "type": "integer"},
        {"field_name": "mantrap_id", "type": "integer"}
    ]
    data, errors = validate_fields(fields, json, cursor)
    if errors:
        release_db(db)
        return {"errors": errors}, 400

    # Get logs
    cursor.execute(
        f"""
        SELECT id, category, user_id, mantrap_id, message, TO_CHAR(timestamp, 'Dy, DD Month YYYY HH24:MI:SS') AS timestamp, file FROM logs
        WHERE {' AND '.join([f'{key} = %s' for key in data.keys()]) if data else 'true'}
        ORDER BY id DESC
        """,
        tuple(data.values())
    )
    logs = cursor.fetchall()

    release_db(db)
    return {"data": logs}


def handle_get_log_file(filename):

    # Filename must have a specific format
    if not match("^[a-z]+_\\d{17}.[a-z]+$", filename):
        return {"message": "Invalid filename"}, 400

    # Check if file exists
    file_path = join("log/alerts", filename)
    if not Path(file_path).is_file():
        return {"message": "Invalid filename"}, 400

    return send_file(file_path)


def handle_verified(payload):
    db, cursor = get_db()
    payload = loads(payload)

    # Get verified username
    username = payload.get("personnel_id")
    if not username:
        print("[MQTT] Verified personnel_id (username) not found")
        return

    # Get user with username
    cursor.execute("SELECT * FROM users WHERE username = %s", (username,))
    user = cursor.fetchone()

    # Invalid username
    if not user:
        print("[MQTT] Invalid verified personnel_id (username)")
        return

    # Add to log
    data = {
        "category": "verification_successful",
        "user_id": user.get("id"),
        "message": f"User {username} verified successfully"
    }
    cursor.execute("INSERT INTO logs (category, user_id, message) VALUES (%s, %s, %s)", tuple(data.values()))
    db.commit()
    release_db(db)


def handle_alert(payload):
    db, cursor = get_db()
    payload = loads(payload)

    # Check valid payload
    message, picture = payload.get("type"), payload.get("image")

    if message not in ("multi", "diff"):
        print("[MQTT] Invalid alert message")
        release_db(db)
        return

    if not picture:
        print("[MQTT] Alert image not found")
        release_db(db)
        return

    # Convert base64 to bytes
    try:
        file_data = b64decode(picture)
    except binascii.Error:
        print("[MQTT] Invalid picture data")
        release_db(db)
        return

    # Save payload image
    filename = f"{message}_{datetime.now().strftime("%Y%m%d%H%M%S%f")[:-3]}.png"
    with open(f"log/alerts/{filename}", "wb") as file:
        file.write(file_data)

    # Add to log
    data = {
        "category": "alert",
        "message": "Multiple people detected in mantrap" if message == "multi" else "Different person detected in mantrap",
        "file": filename
    }
    cursor.execute("INSERT INTO logs (category, message, file) VALUES (%s, %s, %s)", tuple(data.values()))
    db.commit()

    # Get subscribers to alerts
    cursor.execute("SELECT email FROM users WHERE alert_subscribed = true AND role IN ('admin', 'security')")
    emails = [_.get("email") for _ in cursor.fetchall()]
    release_db(db)

    # Send out email
    if emails:
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        templates = {
            "multi": {
                "subject": f"🚨 GateGuard Security Alert - {timestamp}",
                "body": """
                Dear Sir/Madam
                
                The GateGuard Security Mantrap System has detected multiple personnel inside the mantrap enclosure, which may indicate a tailgating event.
                
                Both entry and exit gates have been locked. Please review the image from the alert logs in the GateGuard Security Portal.
                
                GateGuard Security System
                """
            },
            "diff": {
                "subject": f"🚨 GateGuard Security Alert - {timestamp}",
                "body": """
                Dear Sir/Madam
                
                The GateGuard Security Mantrap System has detected a mismatch between the personnel who unlocked the gate and the personnel inside the mantrap enclosure. This may indicate an unauthorised entry attempt.
                
                Both entry and exit gates have been locked. Please review the image from the alert logs in the GateGuard Security Portal.
                
                GateGuard Security System
                """
            }
        }
        template = templates.get(message)
        send_email(emails, template.get("subject"), template.get("body"))
