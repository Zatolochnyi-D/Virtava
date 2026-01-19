from app import App

model = 'face_landmarker.task'
host = 'localhost'
port = 13133

app = App(model, host, port)
app.start()
print('End of program')