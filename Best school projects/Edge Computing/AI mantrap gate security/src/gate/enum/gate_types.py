from enum import Enum, auto


class GateType(Enum):
    """
    Represents different types of gates available in an enumeration.

    This class is an enumeration that provides distinct gate types which can be used
    to categorize or identify specific gates in a system or application. Each member
    of the enumeration represents a unique gate type.

    :cvar GATE1: A gate type representing the first gate option.
    :cvar GATE2: A gate type representing the second gate option.
    """
    GATE1 = auto()
    GATE2 = auto()
