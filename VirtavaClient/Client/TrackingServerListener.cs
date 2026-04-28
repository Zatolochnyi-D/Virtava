using System;
using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;

namespace Virtava.Client
{
    public class TrackingServerListener<T> : IDisposable where T : IMessage<T>, new()
    {
        /// <summary>
        /// Event is fired from a background thread.
        /// </summary>
        public event Action<T>? OnResultReceived;

        private readonly MessageParser<T> _messageParser;
        private readonly NetMQPoller _poller;
        private readonly SubscriberSocket _broadcastSocket;
        private readonly RequestSocket _heartbeatSocket;
        private int _listenerId = -1;
        private Ping _ping = new Ping() { IsLast = false };

        public TrackingServerListener(int broadcastPort, int heartbeatPort, int millisecondsPerPing = 1000)
        {
            _messageParser = new MessageParser<T>(() => new T());
            _poller = new NetMQPoller();

            _broadcastSocket = new SubscriberSocket($"tcp://localhost:{broadcastPort}");
            _broadcastSocket.SubscribeToAnyTopic();
            _broadcastSocket.Options.ReceiveHighWatermark = 1;
            _broadcastSocket.ReceiveReady += (sender, args) => ReceiveBroadcast(args.Socket);

            _heartbeatSocket = new RequestSocket($"tcp://localhost:{heartbeatPort}");
            
            var timer = new NetMQTimer(millisecondsPerPing);
            timer.Elapsed += (sender, args) => DoPing();
            var timer2 = new NetMQTimer(1500);
            timer2.Elapsed += (sender, args) => Console.WriteLine("Ping");

            _poller.Add(_broadcastSocket);
            _poller.Add(timer);
            _poller.Add(timer2);
            _poller.RunAsync();
        }

        private void ReceiveBroadcast(NetMQSocket socket)
        {
            // TODO: test any errors that may happen here.
            var messageBytes = socket.ReceiveFrameBytes();
            var message = _messageParser.ParseFrom(messageBytes);
            OnResultReceived?.Invoke(message);
        }

        private void DoPing()
        {
            Console.WriteLine("Starting ping process");

            _ping.Id = _listenerId;
            _heartbeatSocket.SendFrame(_ping.ToByteArray());
            Console.WriteLine("Sent!");

            _ping = Ping.Parser.ParseFrom(_heartbeatSocket.ReceiveFrameBytes()); // this one blocks btw, if server is offline.
            Console.WriteLine($"Received! {_ping.Id}");

            if (_ping.Id == -1)
            {
                // Server lost track of us. Send new ping with -1 id to notify we are still present and get new id.
                Console.WriteLine("Server lost track of us. Sending new connection request.");
                _ping.IsLast = false;
                _heartbeatSocket.SendFrame(_ping.ToByteArray());
                _ping = Ping.Parser.ParseFrom(_heartbeatSocket.ReceiveFrameBytes());
            }
            _listenerId = _ping.Id;
            // TODO: Force send last ping on disconnect.
        }

        public void Dispose()
        {
            _poller.Dispose();
            _broadcastSocket.Dispose(); // TODO: Make sure socket is closed properly and that there will be no reads afterward.
            _ping.Id = _listenerId;
            _ping.IsLast = true;
            _heartbeatSocket.SendFrame(_ping.ToByteArray()); // This will cause issues if: listener right now tries to send ping; listener right now waits for answering ping.
            _heartbeatSocket.Dispose();
        }
    }
}