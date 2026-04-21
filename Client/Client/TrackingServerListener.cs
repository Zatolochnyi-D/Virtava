using System;
using System.Threading;
using Google.Protobuf;
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
        private int _listenerId = -1;

        public TrackingServerListener(int broadcastPort, int heartbeatPort, int millisecondsPerPing = 1000)
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

            var timer = new NetMQTimer(millisecondsPerPing);
            timer.Elapsed += (sender, args) =>
            {
                var ping = new Ping()
                {
                    Id = Volatile.Read(ref _listenerId),
                    IsLast = false,
                };
                _heartbeatSocket.SendFrame(ping.ToByteArray());
                Console.WriteLine("Sent!");
                ping = Ping.Parser.ParseFrom(_heartbeatSocket.ReceiveFrameBytes());
                Console.WriteLine($"Received! {ping.Id}");
                Volatile.Write(ref _listenerId, ping.Id);
            };

            _poller.Add(_broadcastSocket);
            _poller.Add(timer);
            _poller.RunAsync();
        }

        public void Dispose()
        {
            _poller.Dispose();
            _broadcastSocket.Dispose(); // TODO: Make sure socket is closed properly and that there will be no reads afterward.
        }
    }
}