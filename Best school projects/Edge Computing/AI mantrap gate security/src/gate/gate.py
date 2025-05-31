import logging
import time

from auth.detection.main import IntruderDetector
from auth.face import FaceVerification
from auth.motion import MotionDetector
from auth.voice import VoiceAuth
from electronic.driver import Driver
from gate.enum.gate_types import GateType
from gate.enum.states import GateState
from gate.event_manager import GateCallbackHandler
from gate.state_manager import StateManager
from gate.update_manager import UpdateManager
from network.mqtt.publisher import Publisher
from network.mqtt.subscriber import Subscriber
from utils.logger_mixin import LoggerMixin


class Gate(LoggerMixin):
    """
    Represents a Gate system capable of managing various configurations, such as
    voice authentication, motion detection, face verification, and state management.
    The Gate class interacts with a Publisher and Subscriber for MQTT communication,
    manages updates via an UpdateManager, and processes state changes.

    This class provides functionalities for initializing various components based
    on configuration, handling state transitions, and managing system updates. It
    supports two types of gates (`GateType.GATE1` and `GateType.GATE2`) with distinct
    behaviors and message handling.

    :ivar gate_type: The type of gate, determining its functionality behavior and
        connections.
    :type gate_type: GateType
    :ivar logger: Logger instance for handling logging operations.
    :type logger: logging.Logger
    :ivar publisher: Publisher for publishing MQTT messages.
    :type publisher: Publisher
    :ivar _subscriber: Subscriber for receiving MQTT messages.
    :type _subscriber: Subscriber
    :ivar personnel_id: Identifier for the personnel currently interacting with the
        gate.
    :type personnel_id: Optional[str]
    :ivar is_busy: Tracks if the gate system is in a state of being actively used.
    :type is_busy: bool
    :ivar state_manager: Manages the state transitions and processes states of the
        gate.
    :type state_manager: StateManager
    :ivar update_manager: Manages and processes updates for the gate.
    :type update_manager: UpdateManager
    :ivar callback_handler: Handles callbacks for gate-specific events and messages.
    :type callback_handler: GateCallbackHandler
    :ivar voice_auth: Manages voice authentication functionality if configured.
    :type voice_auth: Optional[VoiceAuth]
    :ivar motion_detector: Handles motion detection functionality when configured.
    :type motion_detector: Optional[MotionDetector]
    :ivar face_verification: Manages face verification functionality to ensure
        identity validation.
    :type face_verification: FaceVerification
    """

    def __init__(
            self,
            gate_type,
            mqtt_config,
            update_config,
            voice_auth_config,
            motion_detector_config,
            face_verification_config,
            intruder_detection_config,
            driver_config,
            logging_level=logging.INFO,
    ):
        """
        Initializes the GateSystem instance with provided configuration and sets up its
        internal components. This class manages the state and behavior of a gate system,
        allowing for modular integration of functionalities such as voice authentication,
        motion detection, and face verification through respective configurations.

        :param gate_type: Type of the gate (e.g., turnstile, sliding gate) to be configured.
        :type gate_type: GateType
        :param mqtt_config: Configuration details for the MQTT broker, enabling messaging
                            and communication between components.
        :type mqtt_config: dict
        :param update_config: Configuration for managing update processes related
                              to the gate system.
        :type update_config: dict
        :param voice_auth_config: Configuration for enabling the voice authentication
                                  module. Optional if voice authentication is not required.
        :type voice_auth_config: dict | None
        :param motion_detector_config: Configuration for enabling the motion detection
                                        module. Optional if motion detection is not required.
        :type motion_detector_config: dict | None
        :param face_verification_config: Configuration for enabling the face verification
                                          module.
        :type face_verification_config: dict
        :param logging_level: Logging verbosity level to control the granularity of
                              logs produced.
        :type logging_level: int
        """
        self._last_logged_state = None
        self.gate_type = gate_type
        self.logger = self.setup_logger(__name__, logging_level)
        self.publisher = Publisher(mqtt_config, logging_level)
        self._subscriber = Subscriber(mqtt_config, logging_level)
        self.personnel_id = None
        self.is_busy = False
        self.state_manager = StateManager(self)
        self.update_manager = UpdateManager(self, update_config)
        self.callback_handler = GateCallbackHandler(self)
        if voice_auth_config:
            self.voice_auth = VoiceAuth(voice_auth_config, logging_level)
        if motion_detector_config:
            self.motion_detector = MotionDetector(motion_detector_config, logging_level)
        self.face_verification = FaceVerification(
            face_verification_config, logging_level
        )
        if intruder_detection_config:
            self.intruder_detector = IntruderDetector(intruder_detection_config, logging_level)
        self.driver = Driver(driver_config, logging_level)

    def _log_state(self, state_number):
        """
        Logs the current system state and updates internal flags based on the given
        state number. This method ensures that the state is logged only when it
        differs from the last logged state. It also determines whether the system
        is busy depending on the state number value.

        :param state_number: The state identifier of the system. If the value is
            None or 1, the system is considered not busy. Otherwise, the system
            is marked as busy.
        :return: None
        """
        if state_number != self._last_logged_state:
            self.logger.info(f"State {state_number}")
            self._last_logged_state = state_number
        # If the state is not None or 1, the system is busy
        self.is_busy = state_number not in [None, 1]

    def process_pending_updates(self):
        """
        Processes all pending updates through the update manager.

        This method delegates the handling of pending updates to the
        update manager and ensures that all updates queued for processing
        are addressed.

        :raises Exception: If the update manager encounters an issue while
            processing updates.
        """
        self.update_manager.process_pending_updates()

    def run(self):
        """
        Establishes connections and manages the behavior of the system based on the
        specified gate type. Subscribes to relevant topics and processes the current
        state of the system.

        :param self: Instance of the class containing system configurations and
            necessary utilities such as publishers, subscribers, and state manager.
        :raises ConnectionError: If the connections to publisher or subscriber cannot
            be established.
        :return: None
        """
        self.publisher.connect()
        self._subscriber.connect()
        self._subscriber.subscribe("update/embedding", 2)

        while not self.publisher.connected or not self._subscriber.connected:
            time.sleep(1)

        if self.gate_type == GateType.GATE1:
            self.state_manager.current_state = GateState.WAITING_FOR_FACE
            self._subscriber.on_message_callback = (
                self.callback_handler.handle_gate1_message
            )
            self._subscriber.subscribe("gate_2/status", 2)
            self._subscriber.subscribe("command", 2)
            self._subscriber.subscribe("alert", 2)

        if self.gate_type == GateType.GATE2:
            self.state_manager.current_state = GateState.IDLE
            self._subscriber.on_message_callback = (
                self.callback_handler.handle_gate2_message
            )
            self._subscriber.subscribe("verified", 2)

        while True:
            self.state_manager.process_current_state()
