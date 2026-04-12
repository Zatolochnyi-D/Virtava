import zmq
from threading import Thread, Event as ThreadEvent
from google.protobuf.message import Message
from virtava_server.connections_tracking_list import ConnectionsTrackingList
from virtava_server.event import Event
from virtava_server.heartbeatMessages_pb2 import Ping

# TODO: as proto messages are in use now, restrict protobuf version
class TrackerServer:  # TODO: add logger maybe? Look up how to use one first.
    def __init__(self, broadcast_port: int, heartbeat_port: int, connection_timeout: int):
        self.first_listener_connected = Event()
        self.no_listeners_left = Event()

        self._context = zmq.Context()
        self._broadcast_socket = self._context.socket(zmq.PUB)
        self._heartbeat_socket = self._context.socket(zmq.REP) # REQ/REP may be not sufficient - strict order of communication may not allow client
                                                               # to notify server about disconnection immediately.

        self._broadcast_socket.bind(f'tcp://localhost:{broadcast_port}')
        self._heartbeat_socket.bind(f'tcp://localhost:{heartbeat_port}')

        self._thread_event = ThreadEvent()
        self._connections = ConnectionsTrackingList()
        self._check_timeouts_thread = Thread(target = self._check_connection_timeouts_indefinitely, args = [connection_timeout])
        self._check_timeouts_thread.start()
        self._heartbeat_thread = Thread(target = self._heartbeat_indefinitely)
        self._heartbeat_thread.start()

    def _heartbeat_indefinitely(self):
        poller = zmq.Poller()
        # poller.regist

        while not self._thread_event.is_set():
            print('awaiting request')
            request = Ping()
            request.ParseFromString(self._heartbeat_socket.recv())
            reply = Ping(isLast = False)
            if request.id == -1:
                print('  Got new client')
                reply.id = self._connections.create_connection()
                self.first_listener_connected.fire()
            elif not self._connections.connection_exists(request.id):
                print('  Got unknown client')
                reply.id = -1
            elif not request.isLast:
                print('  Got known client')
                self._connections.update_connection_timestamp(request.id)
                reply.id = request.id
            else:
                print('  Got disconnecting client')
                if self._connections.remove_connection(request.id) and self._connections.count == 0:
                    self.no_listeners_left.fire()
                reply.id = -1
            self._heartbeat_socket.send(reply.SerializeToString())
        # TODO: pass socket to this thread as an argument, and close it here when server is closed.

    def _check_connection_timeouts_indefinitely(self, connection_timeout: int):
        while True:
            self._thread_event.wait(timeout = connection_timeout)
            if (self._thread_event.is_set()):
                break

            print(self._connections.count)
            print('checking for timed out connections')
            if self._connections.remove_timed_out_connections(connection_timeout) != 0 and self._connections.count == 0:
                print('no clients')
                self.no_listeners_left.fire()
            print(self._connections.count)

    def send(self, result: Message):
        try:
            self._broadcast_socket.send(result.SerializeToString())
        except zmq.ZMQError:
            print('socket closed error') # TODO: look up how to properly detect socket closed on sending.

    def stop(self):
        self._broadcast_socket.close()
        self._heartbeat_socket.close()
        self._thread_event.set()
        self._heartbeat_thread.join()
        self._context.term()

# Make client send close request to server.

# Combine timeout and hearbeat threads into one.
# Try current system with multiple clients.
# Look up if there are better sockets than req/rep. I need to be able to send data without waiting for response.
# Make tracker server properly close REP socket.
