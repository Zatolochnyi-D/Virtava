import cv2
import numpy as np
from mediapipe import Image, ImageFormat
from mediapipe.tasks.python.core.base_options import BaseOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerOptions
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarker
from mediapipe.python.solutions.drawing_utils import draw_landmarks
from mediapipe.python.solutions import drawing_styles as ds
from mediapipe.framework.formats import landmark_pb2 # protobuf generated of mediapipe/framework/formats/landmark.proto in original repo.
from mediapipe.tasks.python.vision.face_landmarker import FaceLandmarkerResult
from mediapipe.tasks.python.components.containers.landmark import NormalizedLandmark
from mediapipe.python.solutions.face_mesh_connections import FACEMESH_TESSELATION, FACEMESH_CONTOURS, FACEMESH_IRISES

def draw_landmarks_on_image(rgb_image, detection_result: FaceLandmarkerResult):
    face_landmarks_list = detection_result.face_landmarks
    annotated_image = np.copy(rgb_image)
  
    for idx in range(len(face_landmarks_list)):
        face_landmarks: list[NormalizedLandmark] = face_landmarks_list[idx]
  
        face_landmarks_proto = landmark_pb2.NormalizedLandmarkList()
        face_landmarks_p = []
        for landmark in face_landmarks:
            norm_landmark_p = landmark_pb2.NormalizedLandmark(x=landmark.x, y=landmark.y, z=landmark.z)
            face_landmarks_p.append(norm_landmark_p)
        face_landmarks_proto.landmark.extend(face_landmarks_p)

        draw_landmarks(
            image = annotated_image,
            landmark_list = face_landmarks_proto,
            connections = FACEMESH_TESSELATION,
            landmark_drawing_spec = None,
            connection_drawing_spec = ds.get_default_face_mesh_tesselation_style())
        draw_landmarks(
            image=annotated_image,
            landmark_list = face_landmarks_proto,
            connections = FACEMESH_CONTOURS,
            landmark_drawing_spec = None,
            connection_drawing_spec = ds.get_default_face_mesh_contours_style())
        draw_landmarks(
            image = annotated_image,
            landmark_list = face_landmarks_proto,
            connections = FACEMESH_IRISES,
            landmark_drawing_spec = None,
            connection_drawing_spec = ds.get_default_face_mesh_iris_connections_style())
  
    return annotated_image

base_options = BaseOptions(model_asset_path = "face_landmarker.task")
options = FaceLandmarkerOptions(base_options = base_options,
                                        output_face_blendshapes = True,
                                        output_facial_transformation_matrixes = True,
                                        num_faces = 1)
detector = FaceLandmarker.create_from_options(options)

camera = cv2.VideoCapture(0)
while camera.isOpened():
    success, image = camera.read()
    if not success:
        print('Image read unsuccessful')
        break

    mp_image = Image(image_format = ImageFormat.SRGB, data = image)
    detection_result = detector.detect(mp_image)

    if len(detection_result.face_blendshapes) != 0:
        print(len(detection_result.face_blendshapes[0]))
        for category in detection_result.face_blendshapes[0]:
            print(category)
            print(category.category_name)
            print(category.score)

    cv2.imshow("Window", draw_landmarks_on_image(mp_image.numpy_view(), detection_result))

    if cv2.waitKey(100) == ord('q'):
        break
    
camera.release()
cv2.destroyAllWindows()