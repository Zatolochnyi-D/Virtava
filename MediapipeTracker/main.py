import cv2
import signal
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from mediapipe import Image, ImageFormat
from virtava_server.tracker_server import TrackerServer
from tracking_results_pb2 import TrackingResult

port = 14210
model_asset_path = 'face_landmarker.task'

print('Start of program')

tracker_server = TrackerServer(port)

base_options = BaseOptions(model_asset_path = model_asset_path)
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
    if detected_faces_count:
        print('  Send success')
        result.trackingSucceded = True
        for category in detection_result.face_blendshapes[0]:
            if category.category_name == '_neutral': continue
            # Add map from different naming conventions to camel case. Use this map to convert category name.
            # result.set_blendshape(ArkitBlendshape.name_to_blendshape_map[category.category_name], category.score)
            setattr(result.blendshapes, category.category_name, category.score)
    else:
        print('  Send failure')
    
    tracker_server.send(result)
    
camera.release()
tracker_server.stop()

print('End of program')