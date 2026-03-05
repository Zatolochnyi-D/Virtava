import cv2
import signal
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from mediapipe import Image, ImageFormat
from serverlib.server import Server
from serverlib.cl_args_handler import ClArgsHandler
from serverlib.tracking_results_builder import TrackingResultsBuilder, ArkitBlendshape

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
    result = TrackingResultsBuilder()
    detected_faces_count = len(detection_result.face_landmarks)
    if detected_faces_count:
        # print('  Send success')
        result.set_tracking_succeded()
        for category in detection_result.face_blendshapes[0]:
            if category.category_name == '_neutral': continue
            # Add map from different naming conventions to camel case. Use this map to convert category name.
            # result.set_blendshape(ArkitBlendshape.name_to_blendshape_map[category.category_name], category.score)
    else:
        pass
        # print('  Send failure')
    
    server.send(result.build())
    
camera.release()
server.stop()

print('End of program')