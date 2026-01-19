from socket import SHUT_RDWR, socket
from threading import Thread
from typing import Callable

class Connection:
    def __init__(self, socket: socket):
        self._running = True
        self._socket = socket
        self._on_close: Callable | None = None
        self._disconnect_thread = Thread(target = self._listen_for_disconnect)
        self._disconnect_thread.start()

    def _listen_for_disconnect(self):
        try:
            while self._running:
                data = self._socket.recv(1024)
                if not data:
                    print("client disconnected")
                    break
        except OSError:
            pass
        if not self._running: # client disconnected
            self.close()

    def set_on_close(self, on_close: Callable):
        self._on_close = on_close

    def close(self, fire_on_close_event = True):
        self._running = False
        self._socket.shutdown(SHUT_RDWR)
        self._socket.close()
        if self._on_close is not None and fire_on_close_event: self._on_close()
        print('Connection closed.')