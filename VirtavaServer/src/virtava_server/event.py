from typing import Callable


class Event:
    def __init__(self):
        self._subscribers: list[Callable] = []

    def subscribe(self, func: Callable):
        self._subscribers.append(func)

    def unsubscribe(self, func: Callable):
        self._subscribers.remove(func)

    def fire(self, *args, **kwargs):
        for func in self._subscribers: func(*args, **kwargs)