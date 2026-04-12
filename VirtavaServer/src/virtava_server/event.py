from typing import Callable


class Event:
    def __init__(self):
        self.__subscribers: list[Callable] = []

    def subscribe(self, func: Callable):
        self.__subscribers.append(func)

    def unsubscribe(self, func: Callable):
        self.__subscribers.remove(func)

    def fire(self, *args, **kwargs):
        for func in self.__subscribers: func(*args, **kwargs)