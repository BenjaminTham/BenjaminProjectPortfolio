from datetime import timedelta
from os import getenv

from flask_socketio import SocketIO, emit
from flask import Flask, session, request
from flask_cors import CORS

from auth.auth import handle_login, handle_logout, require_roles
from biometrics.biometrics import handle_enroll_biometrics, handle_get_embeddings, handle_delete_biometrics
from log.log import handle_get_logs, handle_get_log_file
from mantrap.mantrap import handle_add_mantrap, handle_get_mantraps, handle_update_mantrap, handle_delete_mantrap, handle_command_door
from user.user import handle_get_users, handle_add_user, handle_update_user, handle_delete_user
from utils.mqtt import connect_mqtt

app = Flask(__name__)
app.secret_key = getenv("SECRET_KEY")
socketio = SocketIO(app, cors_allowed_origins="*")
connect_mqtt(socketio)

# Allow cors
CORS(app, origins="*", supports_credentials=True)


@app.before_request
def set_session_timeout():
    session.permanent = True
    app.permanent_session_lifetime = timedelta(minutes=int(getenv("SESSION_TIMEOUT", 30)))


@app.errorhandler(500)
def internal_error(error):
    return {"message": "An error has occurred", "error": str(error)}, 500


@app.route('/')
def hello_world():
    return 'GateGuard API is running! 😁'


@app.route("/api/auth/login", methods=["POST"])
def login():
    return handle_login()


@app.route("/api/auth/logout", methods=["GET"])
def logout():
    return handle_logout()


@app.route("/api/user/list", methods=["POST"])
@require_roles(["admin"])
def get_users():
    return handle_get_users()


@app.route("/api/user/add", methods=["POST"])
@require_roles(["admin"])
def add_user():
    return handle_add_user()


@app.route("/api/user/update/<user_id>", methods=["POST"])
@require_roles(["admin"])
def update_user(user_id):
    return handle_update_user(user_id)


@app.route("/api/user/delete/<user_id>", methods=["DELETE"])
@require_roles(["admin"])
def delete_user(user_id):
    return handle_delete_user(user_id)


@app.route("/api/mantrap/list", methods=["POST"])
@require_roles(["admin", "security"])
def get_mantraps():
    return handle_get_mantraps()


@app.route("/api/mantrap/add", methods=["POST"])
@require_roles(["admin"])
def add_mantrap():
    return handle_add_mantrap()


@app.route("/api/mantrap/update/<mantrap_id>", methods=["POST"])
@require_roles(["admin"])
def update_mantrap(mantrap_id):
    return handle_update_mantrap(mantrap_id)


@app.route("/api/mantrap/delete/<mantrap_id>", methods=["DELETE"])
@require_roles(["admin"])
def delete_mantrap(mantrap_id):
    return handle_delete_mantrap(mantrap_id)


@app.route("/api/mantrap/<mantrap_id>/<action>", methods=["GET"])
@require_roles(["admin", "security"])
def command_door(mantrap_id, action):
    return handle_command_door(mantrap_id, action)


@app.route("/api/biometrics/enroll/<user_id>", methods=["POST"])
@require_roles(["admin"])
def enroll_biometrics(user_id):
    return handle_enroll_biometrics(user_id)


@app.route("/api/biometrics/delete/<user_id>", methods=["DELETE"])
@require_roles(["admin"])
def delete_biometrics(user_id):
    return handle_delete_biometrics(user_id)


@app.route("/api/biometrics/embeddings/<token>/<filename>", methods=["GET"])
def get_embeddings(token, filename):
    return handle_get_embeddings(token, filename)


@app.route("/api/log/list", methods=["POST"])
@require_roles(["admin", "security"])
def get_logs():
    return handle_get_logs()


@app.route("/api/log/file/<filename>", methods=["GET"])
@require_roles(["admin", "security"])
def get_log_file(filename):
    return handle_get_log_file(filename)


@socketio.on("connect", namespace="/api/states/listen")
def handle_connect():
    print(f"Websocket client connected {request.sid}")
    emit("state", "Connection successful")


@socketio.on("disconnect", namespace="/api/states/listen")
def handle_disconnect():
    print("Websocket client disconnected")


if __name__ == '__main__':
    app.run()
