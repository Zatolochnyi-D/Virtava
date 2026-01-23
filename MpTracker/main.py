import asyncio
from app import App

model = 'face_landmarker.task'
host = 'localhost'
port = 13133

async def main():
    app = App(model, host, port)
    await app.start()
    print('End of program')

if __name__ == '__main__':
    asyncio.run(main())