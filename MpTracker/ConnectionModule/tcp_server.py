from socket import socket, AF_INET, SOCK_STREAM, timeout
from threading import Thread
from ConnectionModule.connections_list import ConnectionsList
from ConnectionModule.connection import Connection

class TcpServer:
    SOCKET_TIMEOUT = 1.0

    def __init__(self, host: str, port: int):
        self.running = False
        self._connections = ConnectionsList()
        self._accepting_thread: Thread = None
        self._socket_address = (host, port)
        self._socket: socket = None
        print('TCP server created.')

    def start(self):
        self.running = True
        self._socket = socket(AF_INET, SOCK_STREAM)
        self._socket.settimeout(TcpServer.SOCKET_TIMEOUT)
        self._socket.bind(self._socket_address)
        self._socket.listen()
        self._accepting_thread = Thread(target = self._accept_clients_continuously)
        self._accepting_thread.start()
        print('TCP server started.')

    def stop(self):
        self.running = False
        self._socket.close()
        self._connections.close_all()
        print('TCP server stopped.')

    def _accept_clients_continuously(self):
        while self.running:
            try:
                connection, _ = self._socket.accept()
                self._connections.add(Connection(connection))
            except timeout:
                continue
            except OSError:
                break