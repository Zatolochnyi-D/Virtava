# Virtava Project

Bachelor degree project, a framework for setting up an arbitrary face expression recognition program with a software that may use such technology for virtual avatar animation. The idea is to design a middleware that allows to combine arbitrary tracking and rendering software with minimal changes and compatability issues.

The project uses client-server architecture - tracker and renderer are set up as separate processes and communicate with each other. The tracker acts as a server, broadcasting tracking data, and renderer acts as a client, consming tracking data. Functionality is split in packages - server package to integrate with tracker app and client package to integrate with renderer one. The idea is that packages are easily implemented, allowing to scale the framework to include different texhnologies.

Project's stack is:
- Protocol Buffers: allows user defined data schemes (due to each tracker using their own tracking data format).
- ZeroMQ: allows communication between processes, with ability to choose transport.

Project includes a demo app. This app is built using Mediapipe for Python (tracker) and Unity (renderer).

Project implements Server package for Python, Client package C# and Unity, and data scheme for Mediapipe face tracker output (ARKit Blendshapes).
