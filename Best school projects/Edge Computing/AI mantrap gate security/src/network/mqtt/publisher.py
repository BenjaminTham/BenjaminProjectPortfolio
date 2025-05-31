import logging

import paho.mqtt.client as mqtt

from utils.logger_mixin import LoggerMixin


class Publisher(LoggerMixin):
    """
    The Publisher class facilitates publishing messages to an MQTT broker.

    The Publisher class is designed to provide a structured way of
    interacting with an MQTT broker. It simplifies the connection,
    message publishing, and disconnection processes while enabling
    logging capabilities. The class primarily operates with the provided
    MQTT configuration, allowing support for authentication and logging.

    :ivar _broker: The MQTT broker's address or IP to which the client connects.
    :type _broker: str
    :ivar _port: The port on which the MQTT broker operates.
    :type _port: int
    :ivar _connected: The connection state indicating if the client
        is connected to the broker.
    :type _connected: bool
    :ivar _client: The MQTT client instance used for communication with the broker.
    :type _client: paho.mqtt.client.Client
    :ivar _logger: The logger instance used for logging messages at
        the specified logging level.
    :type _logger: logging.Logger
    """

    def __init__(self, mqtt_config, logging_level=logging.INFO):
        """
        Initializes an MQTT client wrapper with configuration settings and logging capabilities.

        This constructor sets up a client for MQTT communication using the provided configuration.
        The MQTT client will be configured with a broker, port, and optionally with username and
        password for authentication. Callback functions for connection and publishing are also
        configured. Additionally, a logger will be setup for logging MQTT-related activities.

        :param mqtt_config: A dictionary containing MQTT connection settings such as broker, port,
            and optionally username and password for authentication.
        :param logging_level: The logging level for the logger (e.g., logging.INFO, logging.DEBUG).
            Defaults to logging.INFO.
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
        self._client.on_publish = self._on_publish
        self._logger = self.setup_logger(__name__, logging_level)

    @property
    def connected(self):
        """
        Retrieves the connection status of the object.

        This property indicates whether the object is currently connected or not.
        The returned value is evaluated based on the state of the `_connected` attribute.

        :return: `True` if the object is connected, `False` otherwise
        :rtype: bool
        """
        return self._connected

    def _on_connect(self, client, userdata, flags, reason_code, properties):
        """
        Handle the MQTT client's connection event.

        This method is triggered when the client connects to the MQTT broker. It informs
        the connection status and logs the respective messages based on the return code.
        If the connection is successful, it sets the internal connected flag to True. In
        case of failure, it logs an error indicating the return code. Additional messaging
        is logged if the failure is due to authentication.

        :param client: The MQTT client instance that is invoking the callback.
        :type client: paho.mqtt.client.Client
        :param userdata: The private user data as passed to the client during initialization.
        :type userdata: Any
        :param flags: Response flags sent by the broker during node connection.
        :type flags: Dict
        :param reason_code: MQTT connection result code from the broker.
        :type reason_code: Int
        :param properties: MQTT v5.0 properties sent by the broker during connection.
        :type properties: Dict
        :return: None
        :rtype: None
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

    def _on_publish(self, client, userdata, mid, reason_code, properties):
        """
        Handles the event triggered when a message is published, providing details
        about the message and its publication status.

        :param client: The client instance used for publishing.
        :type client: Any
        :param userdata: User-defined data of any type that will be passed to the callback.
        :type userdata: Any
        :param mid: The message ID of the published message.
        :type mid: int
        :param reason_code: The reason code representing the result of the publishing operation.
        :type reason_code: int
        :param properties: Additional properties associated with the published message.
        :type properties: Any
        :return: None
        """
        self._logger.debug(f"Message published with ID: {mid}")

    def connect(self):
        """
        Establishes a connection to the MQTT broker using the specified broker address
        and port. If successful, starts the MQTT client loop to handle incoming and
        outgoing messages. If an exception occurs during connection, logs the error
        and returns False.

        :return: True if the connection is successfully established, False otherwise
        :rtype: bool
        """
        try:
            self._logger.debug(f"Connecting to MQTT broker {self._broker}:{self._port}")
            self._client.connect(self._broker, self._port)
            self._client.loop_start()
        except Exception as e:
            self._logger.exception(f"Connection failed: {e}")
            return False
        return True

    def publish(self, topic, message, qos=0):
        """
        Publishes a message to a specified topic with a given Quality of Service (QoS) level
        using the MQTT protocol. Logs the publishing attempt and outcome, and handles any
        exceptions that may occur during the operation.

        :param topic: The MQTT topic to which the message should be published.
        :type topic: str
        :param message: The payload to send to the specified topic.
        :type message: str
        :param qos: The Quality of Service level for message delivery (optional, defaults to 0).
        :type qos: int, optional
        :return: True if the message was successfully published, False otherwise.
        :rtype: bool
        """
        try:
            self._logger.debug(f"Publishing to {topic} with QoS {qos}")
            result = self._client.publish(topic, message, qos)
            result.wait_for_publish()
            return result.is_published()
        except Exception as e:
            self._logger.exception(f"Publishing failed: {e}")
            return False

    def disconnect(self):
        """
        Disconnects the client from the MQTT broker.

        This method ensures the MQTT client is safely disconnected by stopping
        the loop and properly disconnecting from the broker. This is useful
        for cleanup operations when the client is no longer needed or during
        application shutdown.

        :return: None
        """
        self._logger.info("Disconnecting from MQTT broker")
        self._client.loop_stop()
        self._client.disconnect()


if __name__ == "__main__":
    logger = logging.getLogger(__name__)
    mqtt_config = {
        "broker": "localhost",
        "port": 1883,
        "username": "mosquitto",
        "password": "mosquitto"
    }
    publisher = Publisher(mqtt_config=mqtt_config)
    publisher.connect()
    publisher.publish("test/topic", "Hello, World!")
    publisher.disconnect()
