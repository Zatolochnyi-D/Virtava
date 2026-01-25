from event import Event
import asyncio


class Connection:
    def __init__(self, task: asyncio.Task, writer: asyncio.StreamWriter):
        self.task = task
        self.writer = writer

# allow connection of multiple clients.
# detect client graceful disconnection.
# detect client connection lost.
# properly close existing connections so clients can see when server is closed.
# send data to all the active clients.
class TcpServer:
    def __init__(self, host, port):
        self._host = host
        self._port = port
        self._server: asyncio.Server | None = None
        self._clients: list[Connection | None] = []
        self.on_first_client_connected = Event()
        self.on_last_client_disconnected = Event()
        print('TcpServer created.')

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

        if sum([1 for connection in self._clients if connection is not None]) == 1:
            self.on_first_client_connected.fire()
        
        print('Client connected.')

    async def _check_for_disconnection(self, reader: asyncio.StreamReader, index: int):
        await reader.read()
        self._clients[index] = None
        if sum([1 for connection in self._clients if connection is not None]) == 0:
            self.on_last_client_disconnected.fire()
        print('Client disconnected.')

    async def start(self):
        self._server = await asyncio.start_server(self._handle_client, self._host, self._port)
        await self._server.start_serving()
        print('TcpServer started.')

    def stop(self):
        if self._server is None:
            return
        self._server.close()
        for client in self._clients:
            if client is not None:
                client.task.cancel()
        print('TcpServer stopped.')

    async def broadcast(self, data: bytes):
        tasks = []
        for client in self._clients:
            if client is not None:
                client.writer.write(len(data).to_bytes(4, 'little', signed = True))
                client.writer.write(data)
                tasks.append(asyncio.create_task(client.writer.drain())) 
        await asyncio.gather(*tasks)