import asyncio
from app import App

model = 'face_landmarker.task'
protocol = 'tcp'
host = 'localhost'
port = 13133

import signal
import time
import zmq
from random import randrange
from landmarks_pb2 import NormalizedLandmarkPoint, NormalizedLandmarkPointsList

context = zmq.Context()
socket = context.socket(zmq.PUB)
socket.bind(f'{protocol}://{host}:{port}')

list = NormalizedLandmarkPointsList()
list.points.append(NormalizedLandmarkPoint(x = 1421, y = 1421, z = 1421))
data = list.SerializeToString()

running = True

def stop():
    global running
    running = False

signal.signal(signal.SIGINT, lambda x, y: stop())
signal.signal(signal.SIGTERM, lambda x, y: stop())

while running:
    # zipcode = randrange(1, 100000)
    # temperature = randrange(-80, 135)
    # relhumidity = randrange(10, 60)
    socket.send(data)
    print('data sent')
    time.sleep(0.2)

    # socket.send_string(f"{zipcode} {temperature} {relhumidity}")

socket.close()
context.destroy()
print('End of program')

# async def main():
#     app = App(model, host, port)
#     await app.start()
#     print('End of program')

# if __name__ == '__main__':
#     asyncio.run(main())