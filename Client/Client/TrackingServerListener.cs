using System;
using NetMQ;
using NetMQ.Sockets;

namespace Virtava.Client
{
    public class TrackingServerListener : IDisposable
    {
        /// <summary>
        /// Runs from background thread.
        /// </summary>
        public event Action<TrackingResult>? OnResultReceived; // TODO: check out what SynchronizationContext is.

        private NetMQPoller _poller;
        private SubscriberSocket _broadcastSocket;
        private RequestSocket _heartbeatSocket;

        public TrackingServerListener(int broadcastPort, int heartbeatPort)
        {
            _poller = new NetMQPoller();
            _broadcastSocket = new SubscriberSocket($"tcp://localhost:{broadcastPort}");
            _broadcastSocket.SubscribeToAnyTopic();
            _broadcastSocket.ReceiveReady += (sender, args) =>
            {
                // TODO: test any errors that may happen here.
                var messageBytes = args.Socket.ReceiveFrameBytes();
                var message = TrackingResult.Parser.ParseFrom(messageBytes);
                OnResultReceived?.Invoke(message);
            };
            _heartbeatSocket = new RequestSocket($"tcp://localhost:{heartbeatPort}");
            _heartbeatSocket.ReceiveReady += (sender, args) =>
            {
                
            };
            _poller.Add(_broadcastSocket);
            _poller.RunAsync();
        }

        public void Dispose()
        {
            _poller.Dispose();
            _broadcastSocket.Dispose(); // TODO: Make sure socket is closed properly and that there will be no reads afterward.
        }
    }
}