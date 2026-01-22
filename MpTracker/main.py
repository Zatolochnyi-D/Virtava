import asyncio
import signal
from app import App
from tcp_server import TcpServer2
from landmarks_pb2 import NormalizedLandmarkPoint, NormalizedLandmarkPointsList

model = 'face_landmarker.task'
host = 'localhost'
port = 13133

# app = App(model, host, port)
# app.start()

running = True

def stop():
    global running
    running = False

async def main():
    global running
    
    server = TcpServer2(host, port)
    signal.signal(signal.SIGTERM, lambda x, y: stop())
    signal.signal(signal.SIGINT, lambda x, y: stop())
    
    await server.start()

    data = NormalizedLandmarkPointsList()
    data.points.append(NormalizedLandmarkPoint(x=14.21, y=14.21, z=14.21))
    data = data.SerializeToString()

    while running:
        await asyncio.sleep(0.5)
        # await server.send((1421).to_bytes(4, 'little', signed = True))
        # await server.send(data)
        # print('data sent')
    server.stop()

    print('End of program')

if __name__ == '__main__':
    asyncio.run(main())

