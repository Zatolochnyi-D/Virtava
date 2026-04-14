from typing import Any, Callable
from threading import Thread


def execute_on_new_thread(func: Callable, *args, **kwargs):
    Thread(target = func, args = args, kwargs = kwargs).start()


class InterthreadedEvent:
    def __init__(self, dispatcher: Callable[[Callable[..., None], Any, Any], None] = execute_on_new_thread):
        self.__subscribers: list[Callable] = []
        self.__dispatcher = dispatcher

    def subscribe(self, func: Callable[..., None]):
        self.__subscribers.append(func)

    def unsubscribe(self, func: Callable[..., None]):
        self.__subscribers.remove(func)

    def fire(self, *args, **kwargs):
        for func in self.__subscribers:
            self.__dispatcher(func, *args, **kwargs)