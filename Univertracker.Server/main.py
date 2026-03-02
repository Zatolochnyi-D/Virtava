from app import App

model = 'face_landmarker.task'
host = 'localhost'
port = 13133

print('Start of program')

app = App(model)
app.start(host, port)

print('End of program')