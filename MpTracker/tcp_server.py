from socket import socket, AF_INET, SOCK_STREAM, timeout, SHUT_RDWR
from threading import Thread
from connections_list import ConnectionsList
from connection import Connection
from landmarks_pb2 import NormalizedLandmarkPoint, NormalizedLandmarkPointsList
from event import Event
import asyncio

# allow connection of multiple clients.
# detect client graceful disconnection.
# detect client connection lost.
# properly close existing connections so clients can see when server is closed.
# send data to all the active clients.

class Connection:
    def __init__(self, task: asyncio.Task, writer: asyncio.StreamWriter):
        self.task = task
        self.writer = writer

class TcpServer2:
    def __init__(self, host, port):
        self._host = host
        self._port = port
        self._server: asyncio.Server | None = None
        self._clients: list[Connection | None] = []
        print('TcpServer created.')

    async def start(self):
        self._server = await asyncio.start_server(self._handle_client, self._host, self._port)
        await self._server.start_serving()
        print('TcpServer started.')

    def stop(self):
        if self._server is not None:
            self._server.close()
            for client in self._clients:
                if client is not None:
                    client.task.cancel()
        print('TcpServer stopped.')

    async def send(self, data: bytes):
        tasks = []
        for client in self._clients:
            if client is not None:
                client.writer.write(len(data).to_bytes(4, 'little', signed = True))
                client.writer.write(data)
                tasks.append(asyncio.create_task(client.writer.drain())) 
        await asyncio.gather(*tasks)

    async def _handle_client(self, reader: asyncio.StreamReader, writer: asyncio.StreamWriter):
        new_connection_index = 0
        for i in range(len(self._clients)):
            if self._clients[i] is not None:
                new_connection_index = i
                break
        else:
            self._clients.append(None)
            new_connection_index = len(self._clients) - 1
        
        disconnection_check_task = asyncio.create_task(self._check_for_disconnection(reader, new_connection_index))
        self._clients[new_connection_index] = Connection(disconnection_check_task, writer)
        print('Client connected.')

    async def _check_for_disconnection(self, reader: asyncio.StreamReader, index: int):
        await reader.read()
        self._clients[index] = None
        print('Client disconnected.')



class TcpServer:
    SOCKET_TIMEOUT = 1.0

    def __init__(self, host: str, port: int):
        self.running = False
        self._connections = ConnectionsList()
        self._accepting_thread: Thread = None
        self._socket_address = (host, port)
        self._socket: socket = None
        self.on_connection_created = Event()
        self.on_connection_ended = Event()
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

    def broadcast(self, data: bytes):
        for connection in self._connections:
            connection.send(len(data).to_bytes(4, 'little', signed = True))
            connection.send(data)

    def _accept_clients_continuously(self):
        while self.running:
            try:
                connection, _ = self._socket.accept()
                self._connections.add(Connection(connection))
                if (self._connections.count == 1):
                    self.on_connection_created.fire()
            except timeout:
                continue
            except OSError:
                break