# CV Noname project (making up name in progress)

Project about animating of virtual avatars. Developed as both personal project and university thesis work. The scope includes setting up face tracking (capturing landmarks and eye's iris, calculating blendshapes), transporting data to rendering software, processing tracking data and applying results to 3D model and rendering final animated model to screen.

<!-- Software is developed with layered architecture - tracking software and rendering software are decoupled and communicate using socket connections, allowing to replace any part of software complex if needed and potentially deplay parts on different devices. -->

Current project's stack:
- Mediapipe for Python (face tracking)
- Protocol Buffers (data transport)
- ZeroMQ (IPC)
- Unity (rendering software)