import cv2
import sys
from argparse import ArgumentParser
from pathlib import Path
from typing import Union
from signal import signal, SIGTERM, SIGINT
from threading import Event
from time import time_ns
from cv2_enumerate_cameras import enumerate_cameras
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from mediapipe import Image, ImageFormat
from arkit_blendshapes_pb2 import ArkitBlendshapesResult
from virtava_server import TrackerServer

MAX_RETRIES = 30
port = 14210
heartbeat_port = port + 1
camera_name: Union[str, None] = None
path_to_resources = ''
if getattr(sys, 'frozen', False):
    path_to_resources = Path(sys._MEIPASS)
else:
    path_to_resources = Path(__file__).parent.parent
model_asset_path = path_to_resources / 'face_landmarker.task'

argparser = ArgumentParser()
argparser.add_argument('-p', '--port', help = 'Set port used for communication. Default is 14210.', )
argparser.add_argument('-b', '--heartbeat', help = 'Set port used for heartbeat. Default is port + 1.')
argparser.add_argument('-c', '--camera-name', help = 'Camera name to use for tracking. By default whatever camera will be retrieved first is used.')
args = argparser.parse_args()

if args.port:
    port = args.port
if args.heartbeat:
    heartbeat_port = args.heartbeat
if args.camera_name:
    camera_name = args.camera_name

base_options = BaseOptions(model_asset_path = model_asset_path)
options = FaceLandmarkerOptions(
    base_options = base_options,
    output_face_blendshapes = True,
    output_facial_transformation_matrixes = True,
    num_faces = 1,
)
detector = FaceLandmarker.create_from_options(options)

tracking_event = Event()
stop_event = Event()
def stop():
    global stop_event, tracking_event
    stop_event.set()
    tracking_event.set()

signal(SIGINT, lambda x, y: stop())
signal(SIGTERM, lambda x, y: stop())

def get_camera(camera_name: Union[str, None]) -> cv2.VideoCapture:
    if (camera_name is None):
        return cv2.VideoCapture(0)
    else:
        for camera in enumerate_cameras():
            if (camera.name == camera_name):
                return cv2.VideoCapture(camera.index, camera.backend)

def start_tracking():
    global tracking_event
    tracking_event = Event()
    camera = get_camera(camera_name)
    if (not camera.isOpened()):
        print('No camera with provided index was found.')
        exit(-1)

    did_retries = 0
    while camera.isOpened() and not tracking_event.is_set():
        success, image = camera.read()
        if not success:
            did_retries += 1
        if did_retries == MAX_RETRIES:
            print('Camera stopped producing frames.')
            stop()
            break
        did_retries = 0
        mp_image = Image(image_format = ImageFormat.SRGB, data = image)
        detection_result = detector.detect(mp_image)
        result = ArkitBlendshapesResult()
        detected_faces_count = len(detection_result.face_landmarks)
        if detected_faces_count:
            result.trackingSucceded = True
            for category in detection_result.face_blendshapes[0]:
                if category.category_name == "_neutral":
                    continue
                setattr(result.blendshapes, category.category_name, category.score)
        else:
            result.trackingSucceded = False
        result.timestamp = time_ns()

        if not tracking_event.is_set():
            tracker_server.send(result)

    camera.release()

def stop_tracking():
    global tracking_event
    tracking_event.set()

tracker_server = TrackerServer(port, heartbeat_port)
tracker_server.first_listener_connected.subscribe(start_tracking)
tracker_server.no_listeners_left.subscribe(stop)

stop_event.wait()
tracker_server.stop()