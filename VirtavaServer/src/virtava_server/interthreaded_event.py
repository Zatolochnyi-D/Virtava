from typing import Any, Callable, Optional
from threading import Thread


class InterthreadedEvent:
    def __init__(self, dispatcher: Optional[Callable[[Callable[[Any], Any]], None]] = None):
        self.__subscribers: list[Callable] = []
        self.__dispatcher = dispatcher

    def subscribe(self, func: Callable[[Any], Any]):
        self.__subscribers.append(func)

    def unsubscribe(self, func: Callable[[Any], Any]):
        self.__subscribers.remove(func)

    def fire(self, *args, **kwargs):
        for func in self.__subscribers:
            if self.__dispatcher is not None:
                self.__dispatcher(func(*args, **kwargs))
            else:
                Thread(target = func, args = args, kwargs = kwargs).start()