import cv2
from signal import signal, SIGTERM, SIGINT
from threading import Event
from time import time_ns
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from mediapipe import Image, ImageFormat
from arkit_blendshapes_pb2 import ArkitBlendshapesResult
from virtava_server import TrackerServer

port = 14210
model_asset_path = "face_landmarker.task"

base_options = BaseOptions(model_asset_path = model_asset_path)
options = FaceLandmarkerOptions(
    base_options = base_options,
    output_face_blendshapes = True,
    output_facial_transformation_matrixes = True,
    num_faces = 1,
)
detector = FaceLandmarker.create_from_options(options)

print("Tracker created")

tracking_event = Event()
stop_event = Event()
def stop():
    global stop_event, tracking_event
    stop_event.set()
    tracking_event.set()

signal(SIGINT, lambda x, y: stop())
signal(SIGTERM, lambda x, y: stop())


def start_tracking():
    global tracking_event
    print('tracking loop started.')
    tracking_event = Event()
    camera = cv2.VideoCapture(0)  # TODO: move camera index injection up. User may have several cameras so it should be configurable which one to use.
                                  # TODO: it is not guaranteed for camera to be found. Handle possible error.

    while camera.isOpened() and not tracking_event.is_set():
        success, image = camera.read()  # TODO: look up what can be cause of unsuccessful read and handle those cases. Just break with fail message is not enough.
        if not success:
            print("Image read unsuccessful")
            break
        mp_image = Image(image_format = ImageFormat.SRGB, data = image)
        detection_result = detector.detect(mp_image)
        result = ArkitBlendshapesResult()
        detected_faces_count = len(detection_result.face_landmarks)
        if detected_faces_count:
            print("  Send success")
            result.trackingSucceded = True
            for category in detection_result.face_blendshapes[0]:
                if category.category_name == "_neutral":
                    continue
                setattr(result.blendshapes, category.category_name, category.score)
        else:
            result.trackingSucceded = False
            print("  Send failure")
        result.timestamp = time_ns()

        if not tracking_event.is_set():
            tracker_server.send(result)

    camera.release()
    print('tracking loop stopped.')

def stop_tracking():
    global tracking_event
    print('stop requested.')
    tracking_event.set()

tracker_server = TrackerServer(port, port + 1)
tracker_server.first_listener_connected.subscribe(start_tracking)
tracker_server.no_listeners_left.subscribe(stop_tracking)

stop_event.wait()
tracker_server.stop()

print("End of program")