import zmq
from threading import Thread, Event as ThreadEvent
from google.protobuf.message import Message
from virtava_server.connections_tracking_list import ConnectionsTrackingList
from virtava_server.interthreaded_event import InterthreadedEvent, execute_on_new_thread
from virtava_server.heartbeatMessages_pb2 import Ping

# TODO: as proto messages are in use now, restrict protobuf version <---
class TrackerServer:  # TODO: add logger maybe? Look up how to use one first.
    __POLLING_TIMEOUT = 500

    def __init__(self, broadcast_port: int, heartbeat_port: int, connection_timeout: int, dispatcher = execute_on_new_thread):
        self.first_listener_connected = InterthreadedEvent(dispatcher)
        self.no_listeners_left = InterthreadedEvent(dispatcher)

        self.__context = zmq.Context()

        self.__broadcast_socket = self.__context.socket(zmq.PUB)
        self.__broadcast_socket.bind(f'tcp://localhost:{broadcast_port}')

        heartbeat_socket = self.__context.socket(zmq.REP)
        heartbeat_socket.bind(f'tcp://localhost:{heartbeat_port}')
        self.__heartbeat_thread_cancellation = ThreadEvent()
        self.__heartbeat_thread = Thread(target = self.__heartbeat_indefinitely, args = [heartbeat_socket, connection_timeout])
        self.__heartbeat_thread.start()

    def __heartbeat_indefinitely(self, reply_socket: zmq.SyncSocket, connection_timeout: int):
        poller = zmq.Poller()
        poller.register(reply_socket, zmq.POLLIN)

        connections = ConnectionsTrackingList()

        while True:
            events = dict(poller.poll(TrackerServer.__POLLING_TIMEOUT))

            if self.__heartbeat_thread_cancellation.is_set():
                break

            if reply_socket in events:
                request = Ping()
                request.ParseFromString(reply_socket.recv())

                if request.id == -1:                                    # New listener
                    print('NEW CLIENT')
                    request.id = connections.create_connection()
                    if connections.count == 1:
                        self.first_listener_connected.fire()
                elif not connections.connection_exists(request.id):     # Improper listener (was untracked but doesn't know it)
                    print('UNTRACKED CLIENT')
                    request.id = -1
                elif not request.isLast:                                # Proper listener
                    print('OLD CLIENT')
                    connections.update_connection_timestamp(request.id)
                else:                                                   # Proper listener that is disconnecting
                    print('DISCONNECTING CLIENT')
                    connections.remove_connection(request.id)
                    if connections.count == 0:
                        self.no_listeners_left.fire()

                reply_socket.send(request.SerializeToString())

            if connections.remove_timed_out_connections(connection_timeout) != 0 and connections.count == 0:
                print('EVERYONE LEFT')
                self.no_listeners_left.fire()

        reply_socket.close()
            
    def send(self, result: Message): # TODO: Look up if this thing is good enough. Like, can I just directly pass things to send to socket, ot should I wrap it with thread or something?
        try:
            self.__broadcast_socket.send(result.SerializeToString())
        except zmq.ZMQError:
            print('socket closed error') # TODO: look up how to properly detect socket closed on sending.

    def stop(self):
        self.__broadcast_socket.close()
        self.__heartbeat_thread_cancellation.set()
        self.__heartbeat_thread.join()
        self.__context.term()




# Final TODO: Make on PROPER client send close request to server properly, without interfering with REQ socket cycle.
# Final TODO: Move this lib to proper server.
# Mb make mock server to test actual client faster.