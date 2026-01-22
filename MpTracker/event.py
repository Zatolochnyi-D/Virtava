from typing import Callable

class Event:
    def __init__(self):
        self._callbacks: list[Callable] = []

    def subscribe(self, callback):
        self._callbacks.append(callback)

    def unsubscribe(self, callback):
        self._callbacks.remove(callback)

    def fire(self, *args, **kwargs):
        for callback in self._callbacks:
            callback(*args, **kwargs)