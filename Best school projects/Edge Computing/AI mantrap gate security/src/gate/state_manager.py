import json
from datetime import datetime
from time import sleep

from gate.enum.gate_types import GateType
from gate.enum.states import GateState

from utils.logger_mixin import LoggerMixin


class StateManager(LoggerMixin):
    """
    Manages the state transitions and handlers for a multi-gate security system.

    The StateManager class handles various states of gate operations, such as
    face verification, passage handling, and mantrap scanning. It acts as
    a central controller processing states and triggering appropriate
    handlers and transitions. This class ensures that gates move through
    properly defined workflows to maintain a secure environment.

    :ivar gate: Reference to the gate being managed.
    :type gate: Gate
    :ivar _last_logged_state: Keeps track of the last logged state for efficiency.
    :type _last_logged_state: Optional[GateState]
    :ivar _current_state: The current state of the gate being processed.
    :type _current_state: Optional[GateState]
    :ivar logger: Logger instance configured for the StateManager.
    :type logger: Logger
    :ivar _state_handlers: Mapping of states to their corresponding handler methods.
    :type _state_handlers: Dict[GateState, Callable]

    :raises KeyError: If an unrecognized state is encountered when processing.
    """

    def __init__(self, gate):
        """
        Represents the state handler for a gate, managing various states and their respective
        handlers for processing gate operations.

        Attributes
        ----------
        gate : Gate
            The gate instance being managed, providing access to its properties such
            as logger and state data.
        _last_logged_state : GateState
            Represents the last logged state of the gate to avoid redundant logs.
        _current_state : GateState
            Tracks the current state of the gate for internal state management.
        logger : Logger
            Logger instance used for logging gate activities and state transitions.
        _state_handlers : dict[GateState, Callable]
            A dictionary mapping gate states to their respective handler methods.

        :param gate: The gate object to manage and control state transitions for.
        :type gate: Gate
        """
        self.gate = gate
        self._last_logged_state = None
        self._current_state = None
        self.logger = self.setup_logger(__name__, self.gate.logger.level)

        # Map states to handler methods
        self._state_handlers = {
            GateState.WAITING_FOR_FACE: self._handle_waiting_for_face,
            GateState.VERIFYING_FACE: self._handle_verifying_face,
            GateState.WAITING_FOR_PASSAGE_G1: self._handle_waiting_for_passage_g1,
            GateState.CHECKING_MANTRAP: self._handle_checking_mantrap,
            GateState.MANUAL_OPEN: self._handle_manual_open,
            GateState.ALERT_ACTIVE: self._handle_alert_active,
            GateState.CAPTURE_INTRUDER: self._handle_capture_intruder,
            GateState.VERIFYING_FACE_G2: self._handle_verifying_face_g2,
            GateState.CAPTURE_MISMATCH: self._handle_capture_mismatch,
            GateState.VERIFYING_VOICE: self._handle_verifying_voice,
            GateState.WAITING_FOR_PASSAGE_G2: self._handle_waiting_for_passage_g2,
        }

    @property
    def current_state(self):
        """
        Provides the functionality to retrieve the current state of an instance.
        This attribute gives a read-only access to the private attribute
        `_current_state`, which represents the current state of the object.

        :returns: The current state of the object
        :rtype: Any
        """
        return self._current_state

    @current_state.setter
    def current_state(self, state):
        """
        Sets the current state and logs the state change.

        :param state: The new state to be assigned to the current state.
        :type state: str
        """
        self._current_state = state
        self._log_state_change(state)

    def _log_state_change(self, state):
        """
        Logs the state change if the new state differs from the last logged state and
        updates the is_busy status of the gate based on the current state. The method
        ensures that only significant state transitions are logged, and the internal
        status of the gate's busy condition is correctly managed.

        :param state: The new state to be processed and potentially logged. It can
                      represent any valid state in the gate's state sequence.
        :type state: GateState

        :return: None
        """
        if state != self._last_logged_state:
            self.gate.driver.clear()
            self.logger.info(f"State changed to: {state.name}")
            self.gate.publisher.publish(
                "state",
                json.dumps({"state": state.name}),
                2,
            )
            self._last_logged_state = state

        # If the state is not IDLE or WAITING_FOR_FACE, the system is busy
        self.gate.is_busy = state not in [
            None,
            GateState.IDLE,
            GateState.WAITING_FOR_FACE,
        ]

    def process_current_state(self):
        """
        Processes the current state by checking its validity and invoking the
        corresponding state handler, if applicable. The method ensures that the
        current state is not `None` and matches one of the predefined state handlers
        before executing the corresponding handler function.

        :raises KeyError: If the current state is not found in the list of handlers.
        """
        if (
                self._current_state is not None
                and self._current_state in self._state_handlers
        ):
            self._state_handlers[self._current_state]()

    def _handle_waiting_for_face(self):
        """
        Handles the state where the system is waiting for a face to appear for verification.
        This state involves checking for motion detection and updates the state accordingly
        if motion is detected.

        :return: None
        """
        self.gate.process_pending_updates()
        if self.gate.motion_detector.wait_for_motion():
            self.current_state = GateState.VERIFYING_FACE

    def _handle_verifying_face(self):
        """
        Handles the process of verifying a face using the gate's face verification system.

        This method interacts with a face verification mechanism to authenticate personnel.
        If the verification is successful, it triggers gate opening and publishes the gate's
        status. If the verification fails, it updates the internal state to attempt verification
        again.

        :raises Exception: If there is an issue during face verification or any related
                           subsequent process (e.g., opening the gate, publishing status).
        """
        self.gate.driver.display_text("Verifying face. Please wait...")
        personnel_id = self.gate.face_verification.wait_for_face_and_verify()
        if personnel_id:
            self.gate.personnel_id = personnel_id
            self.gate.driver.open_gate()
            self.gate.publisher.publish(
                "gate_1/status",
                json.dumps({"opened": datetime.now().strftime("%Y-%m-%d %H:%M:%S")}),
                2,
            )
            self.current_state = GateState.WAITING_FOR_PASSAGE_G1
        else:
            self.current_state = GateState.WAITING_FOR_FACE

    def _handle_waiting_for_passage_g1(self):
        """
        Handles the gate operation while waiting for personnel to pass through. This method
        checks whether the personnel has passed, transitions the gate to a closed state upon
        successful passage, and updates the internal state of the gate system.

        :raises ValueError: If the personnel passage state is invalid.
        """
        self.gate.driver.display_text("Face verified. Please enter.")
        if self.gate.driver.personnel_passed():
            self.gate.driver.close_gate()
            self.current_state = GateState.CHECKING_MANTRAP

    def _handle_checking_mantrap(self):
        """
        Handles checking the mantrap process based on the gate type and its state. The function
        broadcasts the current closure status of the gate or determines the appropriate action
        depending on the mantrap scan results. Adjusts the state of the gate as necessary.

        :return: None
        """
        self.gate.driver.display_text("Please wait.")
        if self.gate.gate_type == GateType.GATE2:
            return

        self.gate.publisher.publish(
            "gate_1/status",
            json.dumps({"closed": datetime.now().strftime("%Y-%m-%d %H:%M:%S")}),
            2,
        )

        if self.gate.intruder_detector.run_tracking():
            self.gate.publisher.publish(
                "verified", json.dumps({"personnel_id": self.gate.personnel_id}), 2
            )
            self.current_state = GateState.IDLE  # gate 2 takes over
        else:
            self.current_state = GateState.CAPTURE_INTRUDER

    def _handle_manual_open(self):
        """
        Handles the manual opening mechanism during a state where no automatic transitions
        occur. This function waits for a command callback to determine and initiate
        any state changes.

        :raises RuntimeError: If the command callback fails or is invoked improperly
        """
        # No transitions - waits for command callback to change state
        pass

    def _handle_alert_active(self):
        """
        Handles the active alert state by triggering an alert mechanism (e.g., alert buzzer
        and LED). This function does not handle state transitions, it simply waits for an
        external command callback to modify the current state. Designed to be invoked
        within the state management logic.

        :return: This function does not return any value.
        """
        self.gate.driver.display_text("Intruder detected!")
        self.gate.driver.alert()
        # No transitions - waits for command callback to change state

    def _handle_capture_intruder(self):
        """
        Handles the detection of an intruder by capturing the image, publishing an alert
        message, and transitioning the gate state to active alert. This function ensures
        that upon detection of an intruder, the necessary actions are taken to notify
        the system and safeguard the area.

        :raises RuntimeError: If image capture fails or the alert publishing process
            encounters an exception.
        """
        buffer = self.gate.intruder_detector.capture_intruder()
        self.gate.publisher.publish(
            "alert",
            json.dumps(
                {
                    "type": "multi",
                    "image": buffer if buffer else "Error capturing image.",
                }
            ),
            2,
        )
        self.current_state = GateState.ALERT_ACTIVE

    def _handle_verifying_face_g2(self):
        """
        Handles the verification process for a face on Gate 2. This function waits for the
        face verification process to complete, identifies personnel, and ensures the
        retrieved personnel ID matches the previously captured personnel ID from Gate 1.
        Based on the comparison result, the gate transitions to the appropriate state.

        :param self: Reference to the current instance of the class.

        :raises AnyException: This function does not explicitly handle exceptions and
                              might raise runtime or API-specific exceptions during the
                              verification process.
        :return: None
        """
        self.gate.driver.display_text("Verifying face. Please wait...")
        personnel_id = self.gate.face_verification.wait_for_face_and_verify()
        self.logger.debug(
            f"Gate 2 personnel ID: {personnel_id} vs Gate 1 personnel ID {self.gate.personnel_id} is {personnel_id == self.gate.personnel_id}"
        )
        if personnel_id == self.gate.personnel_id:
            self.current_state = GateState.VERIFYING_VOICE
        else:
            self.current_state = GateState.CAPTURE_MISMATCH

    def _handle_capture_mismatch(self):
        """
        Handles the scenario where a capture mismatch is detected.

        This method is responsible for reacting to a capture mismatch event.
        It retrieves the mismatched capture buffer and sends an alert with the
        relevant information to the designated message publisher, indicating
        a capture difference. Subsequently, it changes the system state to
        `GateState.IDLE`, allowing the next gate to take over.

        :return: None
        """
        buffer = self.gate.face_verification.capture_mismatch()
        self.gate.publisher.publish(
            "alert",
            json.dumps(
                {
                    "type": "diff",
                    "image": buffer if buffer else "Error capturing image.",
                }
            ),
            2,
        )
        self.current_state = GateState.IDLE  # gate 1 take over

    def _handle_verifying_voice(self):
        """
        Handles the process of verifying a user's voice for gate access control.

        This method ensures that user authentication is performed using
        voice recognition. If the voice authentication is successful, it
        triggers the gate opening mechanism and publishes the gate's status
        update. On failure, the gate system transitions to a mismatch
        capture state for further processing.

        :return: None
        """
        retries = 0

        while retries < 3:
            self.gate.driver.display_text("Verifying voice. Please speak.")
            if self.gate.voice_auth.authenticate_user(self.gate.personnel_id):
                self.gate.driver.open_gate()
                self.gate.publisher.publish(
                    "gate_2/status",
                    json.dumps({"opened": datetime.now().strftime("%Y-%m-%d %H:%M:%S")}),
                    2,
                )
                self.current_state = GateState.WAITING_FOR_PASSAGE_G2
                return
            else:
                if retries <= 2:
                    self.gate.driver.display_text("Failed. Please try again.")
                sleep(1)
                self.logger.warning("Voice authentication failed. Retrying...")  # Log warning
                retries += 1

        # If authentication fails after 3 retries, capture the mismatch
        self.current_state = GateState.CAPTURE_MISMATCH

    def _handle_waiting_for_passage_g2(self):
        """
        Handles the process of monitoring the gate state when waiting for personnel to pass
        and takes actions accordingly, such as closing the gate and publishing the status.

        In this method, the current gate state is checked to determine if personnel have
        passed through Gate 2. If they have passed, the gate will be closed, status updates
        will be published, and the system transitions the gate state to idle.

        :raises TypeError: If gate.publisher's publish method is called with incorrect arguments.
        :raises AttributeError: If the "gate" object is not properly initialized.
        :return: None
        """
        self.gate.driver.display_text("Verified. Please enter.")
        if self.gate.driver.personnel_passed():
            self.gate.driver.close_gate()
            self.gate.publisher.publish(
                "gate_2/status",
                json.dumps({"closed": datetime.now().strftime("%Y-%m-%d %H:%M:%S")}),
                2,
            )
            self.current_state = GateState.IDLE
            self.gate.process_pending_updates()
