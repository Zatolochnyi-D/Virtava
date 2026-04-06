import zmq
from google.protobuf.message import Message

class TrackerServer:  # TODO: add logger maybe? Look up how to use one first.
    def __init__(self, port: int):
        self._context = zmq.Context()

        self._broadcast_socket = self._context.socket(zmq.PUB) # TODO: look up how to define channels to which consumers can subscribe.
        self._broadcast_socket.bind(f'tcp://localhost:{port}') # TODO: handle port already in use.
        # TODO: add monitoring of connections happening - when first client connects, start tracking loop. End when last client disconnects.

    def send(self, result: Message):
        try:
            self._broadcast_socket.send(result.SerializeToString())
        except zmq.ZMQError:
            print('socket closed error') # TODO: look up how to properly detect socket closed on sending.

    def stop(self):
        self._broadcast_socket.close()
        self._context.destroy()