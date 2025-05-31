import logging
from time import sleep

import paho.mqtt.client as mqtt

from utils.logger_mixin import LoggerMixin


class Subscriber(LoggerMixin):
    """
    The Subscriber class acts as an MQTT client that connects to a broker,
    subscribes to topics, and handles incoming messages. It integrates
    customizable logging and message handling for enhanced user-defined
    functionalities.

    This class provides a mechanism for subscribing to MQTT topics and
    handling incoming messages either through a user-defined callback or
    internal logging. The class also encapsulates connection and
    subscription management, offering a seamless developer experience.

    :ivar _broker: The MQTT broker hostname or IP address.
    :ivar _port: The port for MQTT communication.
    :ivar _connected: Connection state of the client with the broker.
    :type _connected: bool
    :ivar _client: Instance of the MQTT client.
    :ivar on_message_callback: An optional user-defined callback for handling
        incoming messages.
    :type on_message_callback: callable
    :ivar _logger: Logger instance to manage logging output.
    """

    def __init__(
            self, mqtt_config, on_message_callback=None, logging_level=logging.INFO
    ):
        """
        Initializes a new instance of the MQTT client handler class.

        This class sets up an MQTT client using the provided configuration,
        including broker details and optional authentication credentials. It
        supports a user-defined callback for handling messages, as well as
        custom logging levels.

        :param mqtt_config: Dictionary containing MQTT configuration details,
            including "broker" (str), "port" (int), and optionally "username"
            (str) and "password" (str).
        :param on_message_callback: Optional callable that will be used as a
            callback for processing incoming MQTT messages. Defaults to None.
        :param logging_level: Logging level for the logger instance
            (e.g., logging.INFO, logging.DEBUG). Defaults to logging.INFO.
        """
        self._broker = mqtt_config["broker"]
        self._port = mqtt_config["port"]
        self._connected = False
        self._client = mqtt.Client(mqtt.CallbackAPIVersion.VERSION2)
        if "username" and "password" in mqtt_config:
            self._client.username_pw_set(
                mqtt_config["username"], mqtt_config["password"]
            )
        self._client.on_connect = self._on_connect
        self._client.on_message = self._on_message
        self._client.on_subscribe = self._on_subscribe
        self.on_message_callback = on_message_callback
        self._logger = self.setup_logger(__name__, logging_level)

    @property
    def connected(self):
        """
        Indicates whether the object is in a connected state.

        The `connected` property returns the state of the connectivity of the object.
        It reflects the value of the private attribute `_connected` which represents
        whether the object is currently connected or not.

        :return: The current state of connectivity.
        :rtype: bool
        """
        return self._connected

    def _on_connect(self, client, userdata, flags, reason_code, properties):
        """
        Handles the MQTT client's connection event and sets the connection state
        based on the provided reason code.

        This method is a callback function that is triggered when the client connects
        to the MQTT broker. It logs the connection status and updates the internal
        state indicating whether the connection was successful or not. Additionally,
        this method provides detailed logging if the connection fails, particularly
        for authentication-related errors.

        :param client: MQTT client instance invoking the callback.
        :type client: Any
        :param userdata: Custom user data associated with the client.
        :type userdata: Any
        :param flags: Response flags sent by the broker.
        :type flags: dict
        :param reason_code: The result of the connection attempt. A value of 0 indicates
            success, while non-zero values indicate failure.
        :type reason_code: int
        :param properties: MQTT v5 properties associated with the connection.
        :type properties: Any
        :return: None
        """
        if reason_code == 0:
            self._logger.info("Connected to MQTT broker")
            self._connected = True
        else:
            self._logger.error(f"Failed to connect, return code: {reason_code}")
            if reason_code == 5:  # Authentication failed
                self._logger.error(
                    "Authentication failed - check username and password"
                )

    def _on_message(self, client, userdata, message):
        """
        Handles incoming MQTT messages received by the client and processes the
        payload based on the provided message handler callback. If no callback
        is provided, it logs the content of the received message. Handles both
        textual and binary payloads appropriately by decoding or keeping the raw
        bytes when decoding is not possible.

        :param client: The MQTT client instance responsible for receiving the message.
        :type client: Any
        :param userdata: User-defined data of any type that is passed to the callback.
        :type userdata: Any
        :param message: The MQTT message containing topic, payload, and metadata.
        :type message: MQTTMessage
        :return: None
        """
        if self.on_message_callback:
            try:
                decoded_payload = message.payload.decode()
                self._logger.debug(
                    f"Received message on topic {message.topic}: {decoded_payload}"
                )
            except UnicodeDecodeError:
                # If decode fails, pass the raw bytes
                decoded_payload = None
                self._logger.debug(f"Received binary message on topic {message.topic}")
            self.on_message_callback(message.topic, decoded_payload, message.payload)
        else:
            try:
                self._logger.info(
                    f"Received message on topic {message.topic}: {message.payload.decode()}"
                )
            except UnicodeDecodeError:
                self._logger.info(
                    f"Received binary message on topic {message.topic}: {message.payload!r}"
                )

    def _on_subscribe(self, client, userdata, mid, reason_codes, properties):
        """
        Handles the MQTT subscription confirmation callback.

        This method is triggered once the client completes the subscription
        request to an MQTT topic. It logs the subscription acknowledgment message
        detailing the message ID of the subscription.

        :param client: The MQTT client instance that made the subscription.
        :param userdata: The private user data provided when creating the client instance.
        :param mid: The message ID for the subscription acknowledgment.
        :param reason_codes: The set of reason codes indicating the result of the subscription process.
        :param properties: Additional MQTT properties sent with the subscription acknowledgment.
        :return: None
        """
        self._logger.info(f"Subscribed with message ID: {mid}")

    def connect(self):
        """
        Establishes a connection to the MQTT broker and starts the client loop.

        This function attempts to connect to the specified MQTT broker at the configured
        host and port. If successful, it initiates the client loop for handling communication.
        If an exception occurs during the connection process, it logs the error and returns
        a failure status.

        :raises Exception: Logs any exceptions that occur during the connection process.
        :return: Returns True if the connection is successfully established, otherwise False.
        :rtype: bool
        """
        try:
            self._logger.info(f"Connecting to MQTT broker {self._broker}:{self._port}")
            self._client.connect(self._broker, self._port)
            self._client.loop_start()
        except Exception as e:
            self._logger.exception(f"Connection failed: {e}")
            return False
        return True

    def subscribe(self, topic, qos=0):
        """
        Subscribes to the specified MQTT topic with the given Quality of Service (QoS)
        level. Logs the subscription attempt and handles any exceptions that may
        occur during the process. Returns a boolean indicating success or failure
        of the subscription.

        :param topic: The MQTT topic to subscribe to.
        :type topic: str
        :param qos: The Quality of Service level for the subscription. Default is 0
            (At most once delivery).
        :type qos: int
        :return: True if subscription was successful, False otherwise.
        :rtype: bool
        """
        try:
            self._logger.info(f"Subscribing to topic {topic} with QoS {qos}")
            self._client.subscribe(topic, qos)
        except Exception as e:
            self._logger.exception(f"Subscription failed: {e}")
            return False
        return True

    def disconnect(self):
        """
        Disconnects the client from the MQTT broker.

        This method halts the MQTT client's network loop and disconnects
        the client from the broker. It ensures the MQTT connection is
        terminated gracefully. Proper logging information is recorded
        during the disconnection process.

        :return: None
        """
        self._logger.info("Disconnecting from MQTT broker")
        self._client.loop_stop()
        self._client.disconnect()


if __name__ == "__main__":
    logger = logging.getLogger(__name__)


    # Example usage with custom callback
    def custom_message_handler(topic, text_payload, raw_payload):
        if text_payload is not None:
            logger.debug(
                f"Custom handler received: Topic={topic}, Message={text_payload}"
            )
        else:
            logger.debug(
                f"Custom handler received binary data: Topic={topic}, Raw={raw_payload!r}"
            )


    mqtt_config = {
        "broker": "localhost",
        "port": 1883,
        "username": "mosquitto",
        "password": "mosquitto"
    }

    subscriber = Subscriber(
        mqtt_config=mqtt_config,
        on_message_callback=custom_message_handler,
    )
    subscriber.connect()
    while not subscriber.connected:
        pass
    subscriber.subscribe("test", 2)
    try:
        while True:
            sleep(1)
    except KeyboardInterrupt:
        subscriber.disconnect()
