using System;
using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;

namespace Virtava.Client
{
    public class TrackingServerListener<T> : IDisposable where T : IMessage<T>, new()
    {
        /// <summary>
        /// Runs from background thread.
        /// </summary>
        public event Action<T>? OnResultReceived;

        private readonly MessageParser<T> _messageParser;
        private readonly NetMQPoller _poller;
        private readonly SubscriberSocket _broadcastSocket;
        private readonly RequestSocket _heartbeatSocket;
        private int _listenerId = -1;

        public TrackingServerListener(int broadcastPort, int heartbeatPort, int millisecondsPerPing = 1000)
        {
            _messageParser = new MessageParser<T>(() => new T());
            _poller = new NetMQPoller();

            _broadcastSocket = new SubscriberSocket($"tcp://localhost:{broadcastPort}");
            _broadcastSocket.SubscribeToAnyTopic();
            _broadcastSocket.ReceiveReady += (sender, args) =>
            {
                // TODO: test any errors that may happen here.
                var messageBytes = args.Socket.ReceiveFrameBytes();
                var message = _messageParser.ParseFrom(messageBytes);
                OnResultReceived?.Invoke(message);
            };
            _heartbeatSocket = new RequestSocket($"tcp://localhost:{heartbeatPort}");

            var timer = new NetMQTimer(millisecondsPerPing);
            timer.Elapsed += (sender, args) =>
            {
                var ping = new Ping() // TODO: cache it.
                {
                    Id = _listenerId,
                    IsLast = false,
                };
                _heartbeatSocket.SendFrame(ping.ToByteArray()); // TODO: handle when server started after this send is called. This send will block, and reply will never be sent.
                Console.WriteLine("Sent!");
                ping = Ping.Parser.ParseFrom(_heartbeatSocket.ReceiveFrameBytes());
                Console.WriteLine($"Received! {ping.Id}");
                _listenerId = ping.Id; // TODO: process other scenarios, like id = -1, what means that server no longer knows about us.
            };

            _poller.Add(_broadcastSocket);
            _poller.Add(timer);
            _poller.RunAsync();
        }

        public void Dispose()
        {
            _poller.Dispose();
            _broadcastSocket.Dispose(); // TODO: Make sure socket is closed properly and that there will be no reads afterward.
            _heartbeatSocket.Dispose();
        }
    }
}