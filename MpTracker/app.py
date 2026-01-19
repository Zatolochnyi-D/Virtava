import cv2;
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from mediapipe import Image, ImageFormat
from draw_landmarks import draw_landmarks_on_image
import signal
from ConnectionModule.tcp_server import TcpServer
from threading import Event

class App:
    WINDOW_NAME = 'Tracker Capture'

    def __init__(self, model_asset_path: str, host: str, port: int):
        self._running = True
        self._thread_locker: Event | None = None

        self._tcp_server = TcpServer(host, port)

        base_options = BaseOptions(model_asset_path = model_asset_path)
        options = FaceLandmarkerOptions(base_options = base_options,
                                        output_face_blendshapes = True,
                                        output_facial_transformation_matrixes = True,
                                        num_faces = 1)
        self._detector = FaceLandmarker.create_from_options(options)
        
        signal.signal(signal.SIGINT, lambda x, y: self.stop())
        signal.signal(signal.SIGTERM, lambda x, y: self.stop())

        print('Tracker created.')

    def start(self):
        print('Tracker started.')
        self._tcp_server.start()
        self._thread_locker = Event()
        self._thread_locker.wait()

    def stop(self):
        self._running = False
        self._tcp_server.stop()
        if self._thread_locker is not None: self._thread_locker.set()
        print('Tracker stopped')

    def start_demonstrational_capture(self, camera_index: int):
        camera = cv2.VideoCapture(camera_index)

        while camera.isOpened() and self._running:
            success, image = camera.read()

            if not success:
                print('Image read unsuccessful')
                break

            mp_image = Image(image_format = ImageFormat.SRGB, data = image)
            detection_result = self._detector.detect(mp_image)
            annotated_image = draw_landmarks_on_image(mp_image.numpy_view(), detection_result)

            cv2.imshow(App.WINDOW_NAME, cv2.flip(annotated_image, 1))

        camera.release()
        cv2.destroyAllWindows()


