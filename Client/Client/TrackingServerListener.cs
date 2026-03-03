using System;
using NetMQ;
using NetMQ.Sockets;

namespace Univertracker.Client
{
    public class TrackingServerListener : IDisposable
    {
        /// <summary>
        /// Runs from background thread.
        /// </summary>
        public event Action<TrackingResult>? OnResultReceived; // TODO: check out what SynchronizationContext is.

        private NetMQPoller _poller;
        private SubscriberSocket _socket;

        public TrackingServerListener()
        {
            _poller = new NetMQPoller();
            _socket = new SubscriberSocket("tcp://localhost:13133"); // TODO: move connection string to something more configurable.
            _socket.SubscribeToAnyTopic();
            _socket.ReceiveReady += (sender, args) =>
            {
                // TODO: test any errors that may happen here.
                var messageBytes = args.Socket.ReceiveFrameBytes(); // TODO:  TODO: double check what docs meant under using TryReceive.
                var message = TrackingResult.Parser.ParseFrom(messageBytes);
                OnResultReceived?.Invoke(message);
            };
            _poller.Add(_socket);
            _poller.RunAsync();
        }

        public void Dispose()
        {
            _poller.Dispose();
            _socket.Dispose(); // TODO: Make sure socket is closed properly and that there will be no reads afterward.
        }
    }
}
