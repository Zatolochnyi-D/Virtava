from threading import Event
from signal import signal, SIGINT, SIGTERM
from virtava_server.heartbeatMessages_pb2 import Ping
from virtava_server.tracker_server import TrackerServer

port = 14210

print('server started')
server = TrackerServer(port, port + 1, 5)


lock = Event()
signal(SIGINT, lambda x, y: unlock())
signal(SIGTERM, lambda x, y: unlock())
def unlock():
    global lock
    lock.set()

is_running = False
def start():
    global is_running
    is_running = True
    print('starting sending messages')
    while is_running:
        server.send(Ping(id = 42))
def stop():
    global is_running
    print('stopping sending messages')
    is_running = False

server.first_listener_connected.subscribe(start)
server.no_listeners_left.subscribe(stop)

lock.wait()
server.stop() # throws error
print('program closed')