from enum import Enum, auto


class GateState(Enum):
    """
    Represents various states of a gate control system.

    This enumeration is used to represent the sequential and logical
    states involved in managing the operation of an automated gate
    system. Each state corresponds to a specific step or event in the
    gate's operation process.
    """
    IDLE = auto()  # Was None
    WAITING_FOR_FACE = auto()  # Was 1
    VERIFYING_FACE = auto()  # Was 2
    WAITING_FOR_PASSAGE_G1 = auto()  # Was 3
    CHECKING_MANTRAP = auto()  # Was 4
    MANUAL_OPEN = auto()  # Was 5
    ALERT_ACTIVE = auto()  # Was 6
    CAPTURE_INTRUDER = auto()  # Was 7
    VERIFYING_FACE_G2 = auto()  # Was 8
    CAPTURE_MISMATCH = auto()  # Was 9
    VERIFYING_VOICE = auto()  # Was 10
    WAITING_FOR_PASSAGE_G2 = auto()  # Was 11
