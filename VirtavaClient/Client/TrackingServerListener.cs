using System;
using System.Threading;
using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;

namespace Virtava.Client
{
    public class TrackingServerListener<T> : IDisposable where T : IMessage<T>, new()
    {
        /// <summary> Event is fired on a background thread. </summary>
        public event Action<T>? OnResultReceived;
        public event Action? OnConnectionEstablished; 
        public event Action? OnConnectionLost; // TODO: Implement those.

        private Ping _ping;
        private int _listenerId;

        private readonly SubscriberSocket _broadcastSocket;
        private readonly RequestSocket _heartbeatSocket;
        private readonly MessageParser<T> _messageParser;

        private readonly NetMQPoller _poller;
        private readonly Thread _pollerThread;

        public TrackingServerListener(int broadcastPort, int heartbeatPort, int millisecondsPerPing = 1000)
        {
            Console.WriteLine("Bleh");

            _ping = new Ping { Id = -1, IsLast = false };
            _listenerId = _ping.Id;

            _broadcastSocket = new SubscriberSocket($"tcp://localhost:{broadcastPort}");
            _broadcastSocket.SubscribeToAnyTopic();
            _broadcastSocket.Options.ReceiveHighWatermark = 1;
            _broadcastSocket.ReceiveReady += (sender, args) => ReceiveBroadcast();

            _heartbeatSocket = new RequestSocket($"tcp://localhost:{heartbeatPort}");
            _heartbeatSocket.ReceiveReady += (sender, args) => ReceivePing();

            _messageParser = new MessageParser<T>(() => new T());

            var pingSendingTimer = new NetMQTimer(millisecondsPerPing); // Order is: wait -> action -> wait -> ...
            pingSendingTimer.Elapsed += (sender, args) => SendPing();

            _heartbeatSocket.SendFrame(_ping.ToByteArray());
            Console.WriteLine("Sent initial ping.");

            _poller = new NetMQPoller { _broadcastSocket, _heartbeatSocket, pingSendingTimer };
            _pollerThread = new Thread(_poller.Run);
            _pollerThread.Start();
        }

        private void ReceiveBroadcast()
        {
            var messageBytes = _broadcastSocket.ReceiveFrameBytes();
            var message = _messageParser.ParseFrom(messageBytes); // TODO: We may accept message of a different type from what we expect.
            OnResultReceived?.Invoke(message);
        }

        private void SendPing()
        {
            Console.WriteLine("Sending ping.");

            _ping.Id = _listenerId;
            if (_heartbeatSocket.HasOut) // HasOut actually is PollOut (if socket can send another message. For REQ it returns false after send until receive will be handled.
            {
                _heartbeatSocket.SendFrame(_ping.ToByteArray());
                Console.WriteLine("Ping sent!");
            }
            else
            {
                Console.WriteLine("Ping was not sent.");
            }
        }

        private void ReceivePing()
        {
            Console.WriteLine("Receiving ping.");

            _ping = Ping.Parser.ParseFrom(_heartbeatSocket.ReceiveFrameBytes());

            Console.WriteLine($"Received! {_ping.Id}");

            if (_ping.Id == -1)
            {
                // Server lost track of us. Send new ping with -1 id to notify we are still present and get new id.
                Console.WriteLine("Server lost track of us. Sending new connection request.");
                _ping.IsLast = false;
                _heartbeatSocket.SendFrame(_ping.ToByteArray());
            }

            _listenerId = _ping.Id;
        }

        public void Dispose()
        {
            _poller.Stop();
            _pollerThread.Join();
            Console.WriteLine("Poller thread completely stopped.");

            if (_heartbeatSocket.HasOut) // HasOut is true when server is dead or when Stop was called right in ping sending timer (ping was send but ReceiveReady wasn't processed already).
            {
                _ping.Id = _listenerId;
                _ping.IsLast = true;
                _heartbeatSocket.SendFrame(_ping.ToByteArray()); // If we think server is dead, then no last message for it.
            }

            _broadcastSocket.Dispose();
            _heartbeatSocket.Dispose();

            GC.SuppressFinalize(this);
            Console.WriteLine("Dispose ended!");
        }
    }
}