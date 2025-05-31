import json
from datetime import datetime

from gate.enum.states import GateState

from utils.logger_mixin import LoggerMixin


class GateCallbackHandler(LoggerMixin):
    """
    Handles communication and message processing between gates and dashboards.

    This class acts as a callback handler for processing and managing messages
    received on different topics from Gate 1, Gate 2, and the Dashboard. It
    decodes and routes payloads according to their topics and specified actions,
    updating the gate's state or handling updates as required. The handler depends
    on a gate instance and utilizes its state manager, logger, and update manager
    for operations.

    :ivar gate: Reference to the gate instance to manage state and logs.
    :type gate: Gate
    :ivar _logger: Logger instance used for logging within the handler.
    :type _logger: logging.Logger
    """

    def __init__(self, gate):
        """
        Represents a class responsible for initializing a gate attribute and setting
        up a logger for internal use.

        :param gate: An object that contains a logger attribute required for setting
            up the internal logger.

        :ivar gate: Passed gate object containing required attributes.
        :ivar _logger: Logger instance initialized with the gate's logger.
        """
        self.gate = gate
        self._logger = self.setup_logger(__name__, self.gate.logger.level)

    def handle_gate1_message(self, topic, text_payload, raw_payload):
        """
        Handles incoming messages based on the specified topic and text payload. This method processes
        the message according to its topic, invoking specific helper methods to handle the content
        appropriately. It also logs information about received messages.

        :param topic: The topic of the incoming message that determines the type of processing required.
        :type topic: str
        :param text_payload: The textual content of the incoming message; it is used in direct
                             or JSON-parsed processing.
        :type text_payload: str or None
        :param raw_payload: The raw byte content of the incoming message; it complements the
                            `text_payload` in scenarios requiring raw processing.
        :type raw_payload: bytes
        :return: None
        :rtype: None
        """
        self.gate.logger.info(f"Received message: {text_payload[:20]} from topic: {topic}")
        if text_payload is None:
            return

        if topic == "command":
            self._handle_command(json.loads(text_payload))
        elif topic == "gate_2/status":
            self._handle_gate2_status(json.loads(text_payload))
        elif topic == "alert":
            self._handle_alert(json.loads(text_payload))
        elif topic == "update/embedding":
            self._handle_update_embedding(text_payload)

    def handle_gate2_message(self, topic, text_payload, raw_payload):
        """
        Handles incoming messages with specific topics and processes them accordingly. This method logs
        the received message, determines the topic type, and performs actions based on the topic. If the
        message payload is `None`, the method does not proceed further.

        :param topic: The topic of the message received, which determines the specific action to be taken.
        :type topic: str
        :param text_payload: The textual payload of the message in string format. This may be in JSON
            format and is used for processing based on the topic.
        :param raw_payload: The raw payload of the message provided for processing if required. It can
            contain additional data related to the message.
        :return: None
        """
        self.gate.logger.info(f"Received message: {text_payload[:20]} from topic: {topic}")
        if text_payload is None:
            return

        if topic == "verified":
            self._handle_verified(json.loads(text_payload))
        elif topic == "update/embedding":
            self._handle_update_embedding(text_payload)

    def _handle_command(self, payload):
        """
        Handles a command based on the action specified in the payload.

        This method processes a given payload dictionary to determine the desired
        action and executes corresponding operations accordingly. It adjusts the
        state of the gate based on the successful execution of the specified action.

        :param payload: A dictionary containing the details of the command to be
            executed. The key "action" determines the operation. Supported values are:
            "open" for opening the gate, and "close" for closing the gate.
        :type payload: dict

        :return: None
        """
        if payload["command"] == "open":
            self.gate.driver.open_gate()
            self.gate.state_manager.current_state = GateState.MANUAL_OPEN
            self.gate.publisher.publish(
                "gate_1/status",
                json.dumps({"opened": datetime.now().strftime("%Y-%m-%d %H:%M:%S")}),
                2,
            )
        elif payload["command"] == "close":
            self.gate.driver.close_gate()
            self.gate.state_manager.current_state = GateState.WAITING_FOR_FACE
            self.gate.publisher.publish(
                "gate_1/status",
                json.dumps({"closed": datetime.now().strftime("%Y-%m-%d %H:%M:%S")}),
                2,
            )

    def _handle_gate2_status(self, payload):
        """
        Handles the status of gate2 by updating the current state of the gate based
        on the provided payload. Specifically, if the "closed" key in the payload
        is present and not None, it updates the state to `WAITING_FOR_FACE`.

        :param payload: Dictionary containing the gate2 status data.
        :type payload: dict
        :return: None
        """
        if "closed" in payload and payload["closed"] is not None:
            self.gate.state_manager.current_state = GateState.WAITING_FOR_FACE

    def _handle_alert(self, payload):
        """
        Handles alert logic based on the provided payload.

        This method processes a payload with specific alerting criteria. If the
        payload type is "diff", it updates the current state of the gate in the
        state manager to indicate an active alert condition.

        :param payload: A dictionary containing alert data, with a "type" key
            that determines the action taken.
        :type payload: dict
        :return: None
        """
        if payload["type"] == "diff":
            self.gate.state_manager.current_state = GateState.ALERT_ACTIVE

    def _handle_verified(self, payload):
        """
        Handles the verified payload provided by setting the personnel ID, logging the
        information, and updating the gate's state to VERIFYING_FACE_G2.

        :param payload: The payload dictionary containing the information to process.
        :type payload: dict
        """
        if payload["personnel_id"]:
            self.gate.personnel_id = payload["personnel_id"]
            self.gate.logger.info(f"Personnel ID: {self.gate.personnel_id}")
            self.gate.state_manager.current_state = GateState.VERIFYING_FACE_G2

    def _handle_update_embedding(self, text_payload):
        """
        Handles the process of updating embeddings by parsing a JSON payload and invoking
        the appropriate updating mechanism for valid keys.

        This method ensures that only specific updates (e.g., "face" or "voice") are processed
        and logged for debugging purposes in case of invalid formats.

        :param text_payload: The JSON payload string containing key-value pairs for the update.
        :type text_payload: str
        :return: None
        :raises JSONDecodeError: If the input payload cannot be parsed as valid JSON.
        """
        try:
            payload = json.loads(text_payload)
            updates = {k: v for k, v in payload.items() if k in ["face", "voice"]}
            if updates:
                self.gate.update_manager.handle_update(updates)
        except json.JSONDecodeError:
            self.gate.logger.error("Invalid update command format")
