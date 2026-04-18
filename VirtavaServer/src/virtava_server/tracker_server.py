import logging
import zmq
from threading import Thread, Event as ThreadEvent
from google.protobuf.message import Message
from virtava_server.connections_tracking_list import ConnectionsTrackingList
from virtava_server.interthreaded_event import InterthreadedEvent, execute_on_new_thread
from virtava_server.ping_pb2 import Ping
from virtava_server.exceptions import PortInUseException, ServerClosedException

# Throws PortInUseException when either broadcast_port or heartbeat_port are already used by someone else.
# Throws ServerClosedException when calling send() after stop().
class TrackerServer:
    __POLLING_TIMEOUT = 500

    def __init__(self, broadcast_port: int, heartbeat_port: int, connection_timeout = 5, dispatcher = execute_on_new_thread):
        self.__logger = logging.getLogger(__name__)
        self.__logger.debug("TrackerServer began initialization.")

        self.first_listener_connected = InterthreadedEvent(dispatcher)
        self.no_listeners_left = InterthreadedEvent(dispatcher)

        self.__server_stopped = ThreadEvent()

        self.__context = zmq.Context()

        self.__broadcast_socket = self.__context.socket(zmq.PUB)
        self.__broadcast_socket.setsockopt(zmq.CONFLATE, 1)
        self.__broadcast_socket.setsockopt(zmq.LINGER, 0)
        try:
            self.__broadcast_socket.bind(f'tcp://localhost:{broadcast_port}')
        except zmq.ZMQError as e:
            if e.errno == zmq.EADDRINUSE:
                raise PortInUseException(f'Broadcast port {broadcast_port} is already in use.') from e
            else:
                raise
        
        heartbeat_socket = self.__context.socket(zmq.REP)
        heartbeat_socket.setsockopt(zmq.LINGER, 0)
        try:
            heartbeat_socket.bind(f'tcp://localhost:{heartbeat_port}')
        except zmq.ZMQError as e:
            if e.errno == zmq.EADDRINUSE:
                raise PortInUseException(f'Heartbeat port {heartbeat_port} is already in use.') from e
            else:
                raise
        self.__heartbeat_thread = Thread(target = self.__heartbeat_indefinitely, args = [heartbeat_socket, connection_timeout])
        self.__heartbeat_thread.start()

        self.__logger.info("TrackerServer started.")

    def __heartbeat_indefinitely(self, reply_socket: zmq.SyncSocket, connection_timeout: int):
        self.__logger.debug("Heartbeat cycle started.")

        poller = zmq.Poller()
        poller.register(reply_socket, zmq.POLLIN)

        connections = ConnectionsTrackingList()

        while True:
            events = dict(poller.poll(TrackerServer.__POLLING_TIMEOUT))

            if self.__server_stopped.is_set():
                break

            if reply_socket in events:
                request = Ping()
                request.ParseFromString(reply_socket.recv())

                if request.id == -1:                                    # New listener
                    request.id = connections.create_connection()
                    self.__logger.info("New listener connected. Assigned id - %i.", request.id)
                    if connections.count == 1:
                        self.__logger.info("First listener connected.")
                        self.first_listener_connected.fire()
                elif not connections.connection_exists(request.id):     # Improper listener (was untracked but doesn't know it)
                    self.__logger.warning("Listener with improper id %i sent a ping.", request.id)
                    request.id = -1
                elif not request.isLast:                                # Proper listener
                    self.__logger.debug("Listener with id %i sent a ping.", request.id)
                    connections.update_connection_timestamp(request.id)
                else:                                                   # Proper listener that is disconnecting
                    self.__logger.info("Listener with id %i sent last ping.", request.id)
                    connections.remove_connection(request.id)
                    if connections.count == 0:
                        self.__logger.info("Last listener disconnected.")
                        self.no_listeners_left.fire()

                reply_socket.send(request.SerializeToString())

            if connections.remove_timed_out_connections(connection_timeout) != 0 and connections.count == 0:
                self.__logger.info("All listeners were timed out and disconnected.")
                self.no_listeners_left.fire()

        reply_socket.close()
        self.__logger.debug("Heartbeat cycle ended.")
            
    def send(self, result: Message):
        if not self.__server_stopped.is_set():
            self.__broadcast_socket.send(result.SerializeToString())
        else:
            raise ServerClosedException("Server is closed and cannot send more messages")

    def stop(self):
        self.__logger.debug("Started closing TrackerServer.")
        self.__server_stopped.set()
        self.__broadcast_socket.close()
        self.__heartbeat_thread.join()
        self.__context.term()
        self.__logger.info("TrackerServer closed.")