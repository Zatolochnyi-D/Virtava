import signal
import zmq
import cv2
from typing import Literal, Optional
from threading import Thread, Event
from mediapipe import Image, ImageFormat
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
# from landmarks_pb2 import NormalizedLandmarkPoint, NormalizedLandmarkPointsList
from tracking_results_pb2 import NormalizedLandmark, Blendshapes, TrackingResult

class App: # TODO: add logger maybe? Look up how to use one first.
    def __init__(self, model_asset_path: str):
        self._context = zmq.Context()
        self._socket = self._context.socket(zmq.PUB) # TODO: look up how to define channels to which consumers can subscribe.
        # self._socket.monitor("", zmq.EVE) # TODO: add monitoring of connections happening - when first client connects, start tracking loop. End when last client disconnects.

        self._broadcast_thread: Optional[Thread]  = None
        self._thread_blocker = Event()
        self._running = False

        base_options = BaseOptions(model_asset_path = model_asset_path)
        options = FaceLandmarkerOptions(base_options = base_options,
                                        output_face_blendshapes = True,
                                        output_facial_transformation_matrixes = True,
                                        num_faces = 1)
        self._detector = FaceLandmarker.create_from_options(options)
        
        signal.signal(signal.SIGINT, lambda x, y: self.stop())
        signal.signal(signal.SIGTERM, lambda x, y: self.stop())

        print('App created')

    def _broadcast_continuously(self):
        camera = cv2.VideoCapture(0) # TODO: move camera index injection up.
                                     # TODO: it is not guaranteed for camera to be found. Handle possible error.
                                     # TODO: if system restricts access to camera, app should ask for permission first. OpenCV asks for permission
                                     #       but doesn't wait for user to grant it and fails.
        while camera.isOpened() and self._running:
            success, image = camera.read() # TODO: look up what can be cause of unsuccessful read and handle those causes. Just break with fail message is not enough.
            if not success:
                print('Image read unsuccessful')
                break

            mp_image = Image(image_format = ImageFormat.SRGB, data = image)
            detection_result = self._detector.detect(mp_image)

            # There can be 0 faces detected. In such case no message will be sent. Actually no handling needed on client, in case of no detection
            # the model with just freeze. Can add some timer on client so it can know that server is not sending anything and add custom handlers on
            # no data, or send empty list message and handle it.
            result = TrackingResult()
            detected_faces_count = len(detection_result.face_landmarks)
            if not detected_faces_count:
                result.trackingSucceded = False
                print('  Send failure')
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
                print('  Send success')
                
            try:
                self._socket.send(result.SerializeToString())
            except zmq.ZMQError:
                print('socket closed error') # TODO: look up how to properly detect socket closed on sending.
                                             # TODO: apparently ZMQ sockets are not thread-safe, and I should use send on the same thread where socket is
                                             #       created. So look into polling, non-blocking sockets and queues.
            
        camera.release()

    def start(self, host: str, port: int, protocol: Literal['tcp'] = 'tcp'):
        self._running = True
        self._socket.bind(f'{protocol}://{host}:{port}')
        self._broadcast_thread = Thread(target=self._broadcast_continuously)
        self._broadcast_thread.start()
        print('App started')
        self._thread_blocker.wait()

    def stop(self):
        self._running = False
        self._socket.close()
        self._context.destroy()
        self._thread_blocker.set()