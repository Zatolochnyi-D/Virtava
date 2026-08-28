# Virtava Project

Bachelor degree project, a framework for setting up an arbitrary face expression recognition program with a software that may use such technology for virtual avatars animation. The idea is to design a middleware that allows to combine arbitrary tracking and rendering software with minimal changes and compatability issues.

The project uses client-server architecture - tracker and renderer are set up as separate processes and communicate with each other. The tracker acts as a server, broadcasting tracking data, and renderer acts as a client, consuming tracking data. Functionality is split in packages - server package to integrate with tracker app and client package to integrate with renderer one. The idea is that packages are easily implemented, allowing to scale the framework to include different technologies. The project is designed around desktop platforms, but it should be possible to support mobile as well (as long as used tracker and renderer support them).

Project's stack is:
- Protocol Buffers: allows user defined data schemes (due to each tracker using their own tracking data format).
- ZeroMQ: allows communication between processes, with ability to choose transport.

The project includes a demo app. It is built using Mediapipe for Python (tracker) and Unity (renderer). This app is built for Windows platforms.

The project implements Server package for Python, Client package C# and Unity, and data scheme for Mediapipe face tracker output (ARKit Blendshapes).
