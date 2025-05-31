from json import loads
from os import getenv

from paho.mqtt.client import Client

from log.log import handle_verified, handle_alert
from mantrap.mantrap import handle_gate_status


def on_connect(client, userdata, flags, rc):
    if rc != 0:
        raise Exception("Connection failed")
    print("Connected to MQTT Broker")
    for topic in ["gate_1/status", "gate_2/status", "verified", "alert", "state"]:
        client.subscribe(topic)


def on_message(client, userdata, message):
    topic, payload = message.topic, message.payload
    print(f"[MQTT] Received message from {topic}: {payload if topic != "alert" else str(payload)[:180]}")
    match topic:
        case "gate_1/status" | "gate_2/status":
            handle_gate_status(topic, payload)
        case "verified":
            handle_verified(payload)
        case "alert":
            handle_alert(payload)
        case "state":
            if socketio_:
                # Assume payload is a byte string from MQTT, e.g., b'{"state":"IDLE"}'
                payload_str = payload.decode("utf-8")  # Convert bytes to string
                payload_json = loads(payload_str)  # Parse JSON string into dictionary
                # state = payload_json.get("state")  # Extract "state"
                socketio_.emit("state", payload_json, namespace="/api/states/listen")
                # socketio_.start_background_task(socketio_.emit, "state", payload.decode("utf-8"), namespace="/api/states/listen")


mqtt_client = Client(client_id="fog")
mqtt_client.username_pw_set(getenv("MQTT_USERNAME"), getenv("MQTT_PASSWORD"))
mqtt_client.on_connect = on_connect
mqtt_client.on_message = on_message
socketio_ = None

def connect_mqtt(socketio):
    global socketio_
    socketio_ = socketio
    mqtt_client.connect(getenv("MQTT_BROKER"), int(getenv("MQTT_PORT")), 60)
    mqtt_client.loop_start()

def publish_mqtt(topic, payload):
    mqtt_client.publish(topic, payload, 2)
