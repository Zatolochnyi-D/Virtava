from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class Ping(_message.Message):
    __slots__ = ("id", "isLast")
    ID_FIELD_NUMBER: _ClassVar[int]
    ISLAST_FIELD_NUMBER: _ClassVar[int]
    id: int
    isLast: bool
    def __init__(self, id: _Optional[int] = ..., isLast: bool = ...) -> None: ...
