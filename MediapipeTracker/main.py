import cv2
import signal
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from mediapipe import Image, ImageFormat
from server.cl_args_handler import ClArgsHandler
from server.server import Server
from server.tracking_results_pb2 import NormalizedLandmark, TrackingResult

print('Start of program')

parameters = ClArgsHandler()

server = Server(parameters.port)

base_options = BaseOptions(model_asset_path = parameters.model_asset_path)
options = FaceLandmarkerOptions(base_options = base_options,
                                output_face_blendshapes = True,
                                output_facial_transformation_matrixes = True,
                                num_faces = 1)
detector = FaceLandmarker.create_from_options(options)

running = True
def stop():
    global running
    running = False
signal.signal(signal.SIGINT, lambda x, y: stop())
signal.signal(signal.SIGTERM, lambda x, y: stop())


camera = cv2.VideoCapture(0) # TODO: move camera index injection up.
                             # TODO: it is not guaranteed for camera to be found. Handle possible error.
                             # TODO: if system restricts access to camera, app should ask for permission first. OpenCV asks for permission
                             #       but doesn't wait for user to grant it and fails.
while camera.isOpened() and running:
    success, image = camera.read() # TODO: look up what can be cause of unsuccessful read and handle those cases. Just break with fail message is not enough.
    if not success:
        print('Image read unsuccessful')
        break
    mp_image = Image(image_format = ImageFormat.SRGB, data = image)
    detection_result = detector.detect(mp_image)
    result = TrackingResult()
    detected_faces_count = len(detection_result.face_landmarks)
    if not detected_faces_count:
        result.trackingSucceded = False
        # print('  Send failure')
    else:
        result.trackingSucceded = True
        for detected_landmark in detection_result.face_landmarks[0]:
            landmark = NormalizedLandmark(x = detected_landmark.x, y = detected_landmark.y, z = detected_landmark.z)
            result.normalizedLandmarkList.append(landmark)
        for category in detection_result.face_blendshapes[0]:
            if category.category_name == '_neutral': continue
            setattr(result.blendshapes, category.category_name, category.score) # TODO: not very safe thing, only reliable of belief that blendshape
                                                                                #       names from documentation are the same inside detection results.
                                                                                #       Find a better way to do it.
        # print('  Send success')
    
    server.send(result)
    
camera.release()
server.stop()

print('End of program')