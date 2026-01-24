import cv2
import signal
import asyncio
from mediapipe import Image, ImageFormat
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from tcp_server import TcpServer
from landmarks_pb2 import NormalizedLandmarkPoint, NormalizedLandmarkPointsList

class App:
    def __init__(self, model_asset_path: str, host: str, port: int):
        self._tcp_server = TcpServer(host, port)
        self._tcp_server.on_first_client_connected.subscribe(self.start_capture)
        self._tcp_server.on_last_client_disconnected.subscribe(self.stop_capture)

        self._capturing_task: asyncio.Task | None = None
        self._app_blocker = asyncio.Event()
        self._is_capturing_running = False

        base_options = BaseOptions(model_asset_path = model_asset_path)
        options = FaceLandmarkerOptions(base_options = base_options,
                                        output_face_blendshapes = True,
                                        output_facial_transformation_matrixes = True,
                                        num_faces = 1)
        self._detector = FaceLandmarker.create_from_options(options)

        signal.signal(signal.SIGINT, lambda x, y: self.stop())
        signal.signal(signal.SIGTERM, lambda x, y: self.stop())
        print('Tracker created.')

    async def start(self):
        await self._tcp_server.start()
        print('Application started.')
        await self._app_blocker.wait()

    def start_capture(self):
        self._capturing_task = asyncio.create_task(self.capture_loop())
        self._is_capturing_running = True

    def stop_capture(self):
        if self._capturing_task is not None:
            self._capturing_task.cancel()
            self._capturing_task = None
            self._is_capturing_running = False

    async def capture_loop(self):
        camera = cv2.VideoCapture(0)
        while camera.isOpened() and self._is_capturing_running:
            success, image = camera.read()
            if not success:
                print('Image read unsuccessful')
                break

            mp_image = Image(image_format = ImageFormat.SRGB, data = image)
            detection_result = self._detector.detect(mp_image)

            # There can be 0 faces detected. In such case no message will be sent. Actually no handling needed on client, in case of no detection
            # the model with just freeze. Can add some timer on client so it can know that server is not sending anything and add custom handlers on
            # no data, or send empty list message and handle it.
            detected_faces_count = len(detection_result.face_landmarks)
            landmarkList = NormalizedLandmarkPointsList()
            if detected_faces_count:
                for detected_landmark in detection_result.face_landmarks[0]:
                    landmark = NormalizedLandmarkPoint(x=detected_landmark.x, y=detected_landmark.y, z=detected_landmark.z)
                    landmarkList.points.append(landmark)
            await self._tcp_server.broadcast(landmarkList.SerializeToString())
            
        camera.release()
        cv2.destroyAllWindows()

    def stop(self):
        self._tcp_server.stop()
        self.stop_capture()
        asyncio.get_running_loop().call_soon_threadsafe(self._app_blocker.set)
        print('Application stopped.')