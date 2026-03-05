from enum import Enum
from serverlib.tracking_results_pb2 import TrackingResult

class ArkitBlendshape(Enum):
    BROW_DOWN_LEFT = 1,
    BROW_DOWN_RIGHT = 2,
    BROW_INNER_UP = 3,
    BROW_OUTER_UP_LEFT = 4,
    BROW_OUTER_UP_RIGHT = 5,
    CHEEK_PUFF = 6,
    CHEEK_SQUINT_LEFT = 7,
    CHEEK_SQUINT_RIGHT = 8,
    EYE_BLINK_LEFT = 9,
    EYE_BLINK_RIGHT = 10,
    EYE_LOOK_DOWN_LEFT = 11,
    EYE_LOOK_DOWN_RIGHT = 12,
    EYE_LOOK_IN_LEFT = 13,
    EYE_LOOK_IN_RIGHT = 14,
    EYE_LOOK_OUT_LEFT = 15,
    EYE_LOOK_OUT_RIGHT = 16,
    EYE_LOOK_UP_LEFT = 17,
    EYE_LOOK_UP_RIGHT = 18,
    EYE_SQUINT_LEFT = 19,
    EYE_SQUINT_RIGHT = 20,
    EYE_WIDE_LEFT = 21,
    EYE_WIDE_RIGHT = 22,
    JAW_FORWARD = 23,
    JAW_LEFT = 24,
    JAW_OPEN = 25,
    JAW_RIGHT = 26,
    MOUTH_CLOSE = 27,
    MOUTH_DIMPLE_LEFT = 28,
    MOUTH_DIMPLE_RIGHT = 29,
    MOUTH_FROWN_LEFT = 30,
    MOUTH_FROWN_RIGHT = 31,
    MOUTH_FUNNEL = 32,
    MOUTH_LEFT = 33,
    MOUTH_LOWER_DOWN_LEFT = 34,
    MOUTH_LOWER_DOWN_RIGHT = 35,
    MOUTH_PRESS_LEFT = 36,
    MOUTH_PRESS_RIGHT = 37,
    MOUTH_PUCKER = 38,
    MOUTH_RIGHT = 39,
    MOUTH_ROLL_LOWER = 40,
    MOUTH_ROLL_UPPER = 41,
    MOUTH_SHRUG_LOWER = 42,
    MOUTH_SHRUG_UPPER = 43,
    MOUTH_SMILE_LEFT = 44,
    MOUTH_SMILE_RIGHT = 45,
    MOUTH_STRETCH_LEFT = 46,
    MOUTH_STRETCH_RIGHT = 47,
    MOUTH_UPPER_UP_LEFT = 48,
    MOUTH_UPPER_UP_RIGHT = 49,
    NOSE_SNEER_LEFT = 50,
    NOSE_SNEER_RIGHT = 51,
    TONGUE_OUT = 52,

    name_to_blendshape_map = {
        'browDownLeft': BROW_DOWN_LEFT,
        'browDownRight': BROW_DOWN_RIGHT,
        'browInnerUp': BROW_INNER_UP,
        'browOuterUpLeft': BROW_OUTER_UP_LEFT,
        'browOuterUpRight': BROW_OUTER_UP_RIGHT,
        'cheekPuff': CHEEK_PUFF,
        'cheekSquintLeft': CHEEK_SQUINT_LEFT,
        'cheekSquintRight': CHEEK_SQUINT_RIGHT,
        'eyeBlinkLeft': EYE_BLINK_LEFT,
        'eyeBlinkRight': EYE_BLINK_RIGHT,
        'eyeLookDownLeft': EYE_LOOK_DOWN_LEFT,
        'eyeLookDownRight': EYE_LOOK_DOWN_RIGHT,
        'eyeLookInLeft': EYE_LOOK_IN_LEFT,
        'eyeLookInRight': EYE_LOOK_IN_RIGHT,
        'eyeLookOutLeft': EYE_LOOK_OUT_LEFT,
        'eyeLookOutRight': EYE_LOOK_OUT_RIGHT,
        'eyeLookUpLeft': EYE_LOOK_UP_LEFT,
        'eyeLookUpRight': EYE_LOOK_UP_RIGHT,
        'eyeSquintLeft': EYE_SQUINT_LEFT,
        'eyeSquintRight': EYE_SQUINT_RIGHT,
        'eyeWideLeft': EYE_WIDE_LEFT,
        'eyeWideRight': EYE_WIDE_RIGHT,
        'jawForward': JAW_FORWARD,
        'jawLeft': JAW_LEFT,
        'jawOpen': JAW_OPEN,
        'jawRight': JAW_RIGHT,
        'mouthClose': MOUTH_CLOSE,
        'mouthDimpleLeft': MOUTH_DIMPLE_LEFT,
        'mouthDimpleRight': MOUTH_DIMPLE_RIGHT,
        'mouthFrownLeft': MOUTH_FROWN_LEFT,
        'mouthFrownRight': MOUTH_FROWN_RIGHT,
        'mouthFunnel': MOUTH_FUNNEL,
        'mouthLeft': MOUTH_LEFT,
        'mouthLowerDownLeft': MOUTH_LOWER_DOWN_LEFT,
        'mouthLowerDownRight': MOUTH_LOWER_DOWN_RIGHT,
        'mouthPressLeft': MOUTH_PRESS_LEFT,
        'mouthPressRight': MOUTH_PRESS_RIGHT,
        'mouthPucker': MOUTH_PUCKER,
        'mouthRight': MOUTH_RIGHT,
        'mouthRollLower': MOUTH_ROLL_LOWER,
        'mouthRollUpper': MOUTH_ROLL_UPPER,
        'mouthShrugLower': MOUTH_SHRUG_LOWER,
        'mouthShrugUpper': MOUTH_SHRUG_UPPER,
        'mouthSmileLeft': MOUTH_SMILE_LEFT,
        'mouthSmileRight': MOUTH_SMILE_RIGHT,
        'mouthStretchLeft': MOUTH_STRETCH_LEFT,
        'mouthStretchRight': MOUTH_STRETCH_RIGHT,
        'mouthUpperUpLeft': MOUTH_UPPER_UP_LEFT,
        'mouthUpperUpRight': MOUTH_UPPER_UP_RIGHT,
        'noseSneerLeft': NOSE_SNEER_LEFT,
        'noseSneerRight': NOSE_SNEER_RIGHT,
        'tongueOut': TONGUE_OUT,
    }   


class TrackingResultsBuilder:
    def __init__(self):
        self._results = TrackingResult()
        self._results.trackingSucceded = False

    def set_tracking_succeded(self):
        self._results.trackingSucceded = True

    def set_blendshape(self, blendshape: ArkitBlendshape, value: float):
        if blendshape == ArkitBlendshape.BROW_DOWN_LEFT:
            self._results.browDownLeft = value
        elif blendshape == ArkitBlendshape.BROW_DOWN_RIGHT:
            self._results.browDownRight = value
        elif blendshape == ArkitBlendshape.BROW_INNER_UP:
            self._results.browInnerUp = value
        elif blendshape == ArkitBlendshape.BROW_OUTER_UP_LEFT:
            self._results.browOuterUpLeft = value
        elif blendshape == ArkitBlendshape.BROW_OUTER_UP_RIGHT:
            self._results.browOuterUpRight = value
        elif blendshape == ArkitBlendshape.CHEEK_PUFF:
            self._results.cheekPuff = value
        elif blendshape == ArkitBlendshape.CHEEK_SQUINT_LEFT:
            self._results.cheekSquintLeft = value
        elif blendshape == ArkitBlendshape.CHEEK_SQUINT_RIGHT:
            self._results.cheekSquintRight = value
        elif blendshape == ArkitBlendshape.EYE_BLINK_LEFT:
            self._results.eyeBlinkLeft = value
        elif blendshape == ArkitBlendshape.EYE_BLINK_RIGHT:
            self._results.eyeBlinkRight = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_DOWN_LEFT:
            self._results.eyeLookDownLeft = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_DOWN_RIGHT:
            self._results.eyeLookDownRight = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_IN_LEFT:
            self._results.eyeLookInLeft = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_IN_RIGHT:
            self._results.eyeLookInRight = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_OUT_LEFT:
            self._results.eyeLookOutLeft = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_OUT_RIGHT:
            self._results.eyeLookOutRight = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_UP_LEFT:
            self._results.eyeLookUpLeft = value
        elif blendshape == ArkitBlendshape.EYE_LOOK_UP_RIGHT:
            self._results.eyeLookUpRight = value
        elif blendshape == ArkitBlendshape.EYE_SQUINT_LEFT:
            self._results.eyeSquintLeft = value
        elif blendshape == ArkitBlendshape.EYE_SQUINT_RIGHT:
            self._results.eyeSquintRight = value
        elif blendshape == ArkitBlendshape.EYE_WIDE_LEFT:
            self._results.eyeWideLeft = value
        elif blendshape == ArkitBlendshape.EYE_WIDE_RIGHT:
            self._results.eyeWideRight = value
        elif blendshape == ArkitBlendshape.JAW_FORWARD:
            self._results.jawForward = value
        elif blendshape == ArkitBlendshape.JAW_LEFT:
            self._results.jawLeft = value
        elif blendshape == ArkitBlendshape.JAW_OPEN:
            self._results.jawOpen = value
        elif blendshape == ArkitBlendshape.JAW_RIGHT:
            self._results.jawRight = value
        elif blendshape == ArkitBlendshape.MOUTH_CLOSE:
            self._results.mouthClose = value
        elif blendshape == ArkitBlendshape.MOUTH_DIMPLE_LEFT:
            self._results.mouthDimpleLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_DIMPLE_RIGHT:
            self._results.mouthDimpleRight = value
        elif blendshape == ArkitBlendshape.MOUTH_FROWN_LEFT:
            self._results.mouthFrownLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_FROWN_RIGHT:
            self._results.mouthFrownRight = value
        elif blendshape == ArkitBlendshape.MOUTH_FUNNEL:
            self._results.mouthFunnel = value
        elif blendshape == ArkitBlendshape.MOUTH_LEFT:
            self._results.mouthLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_LOWER_DOWN_LEFT:
            self._results.mouthLowerDownLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_LOWER_DOWN_RIGHT:
            self._results.mouthLowerDownRight = value
        elif blendshape == ArkitBlendshape.MOUTH_PRESS_LEFT:
            self._results.mouthPressLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_PRESS_RIGHT:
            self._results.mouthPressRight = value
        elif blendshape == ArkitBlendshape.MOUTH_PUCKER:
            self._results.mouthPucker = value
        elif blendshape == ArkitBlendshape.MOUTH_RIGHT:
            self._results.mouthRight = value
        elif blendshape == ArkitBlendshape.MOUTH_ROLL_LOWER:
            self._results.mouthRollLower = value
        elif blendshape == ArkitBlendshape.MOUTH_ROLL_UPPER:
            self._results.mouthRollUpper = value
        elif blendshape == ArkitBlendshape.MOUTH_SHRUG_LOWER:
            self._results.mouthShrugLower = value
        elif blendshape == ArkitBlendshape.MOUTH_SHRUG_UPPER:
            self._results.mouthShrugUpper = value
        elif blendshape == ArkitBlendshape.MOUTH_SMILE_LEFT:
            self._results.mouthSmileLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_SMILE_RIGHT:
            self._results.mouthSmileRight = value
        elif blendshape == ArkitBlendshape.MOUTH_STRETCH_LEFT:
            self._results.mouthStretchLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_STRETCH_RIGHT:
            self._results.mouthStretchRight = value
        elif blendshape == ArkitBlendshape.MOUTH_UPPER_UP_LEFT:
            self._results.mouthUpperUpLeft = value
        elif blendshape == ArkitBlendshape.MOUTH_UPPER_UP_RIGHT:
            self._results.mouthUpperUpRight = value
        elif blendshape == ArkitBlendshape.NOSE_SNEER_LEFT:
            self._results.noseSneerLeft = value
        elif blendshape == ArkitBlendshape.NOSE_SNEER_RIGHT:
            self._results.noseSneerRight = value
        elif blendshape == ArkitBlendshape.TONGUE_OUT:
            self._results.tongueOut = value

    def build(self):
        return self._results