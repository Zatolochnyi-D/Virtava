using System;
using NetMQ;
using NetMQ.Sockets;

namespace Univertracker.Client
{
    public class Tracker : IDisposable
    {
        /// <summary>
        /// Runs from background thread.
        /// </summary>
        public event Action<TrackingResult>? OnResultReceived; // TODO: check out what SynchronizationContext is.

        private NetMQPoller _poller;
        private SubscriberSocket _socket;

        public Tracker()
        {
            _poller = new NetMQPoller();
            _socket = new SubscriberSocket("tcp://localhost:13133");
            _socket.SubscribeToAnyTopic();
            _socket.ReceiveReady += (sender, args) =>
            {
                // TODO: test any errors that may happend here.
                var messageBytes = args.Socket.ReceiveFrameBytes(); // TODO: 
                var message = TrackingResult.Parser.ParseFrom(messageBytes);
                OnResultReceived?.Invoke(message);
            };
            _poller.Add(_socket);
            _poller.RunAsync();
        }

        public void Dispose()
        {
            _poller.Dispose();
            _socket.Dispose();
        }
    }
}
