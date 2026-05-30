using System;
using System.Threading;
using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Virtava.Client
{
    public class TrackingServerListener<T> : IDisposable where T : IMessage<T>, new()
    {
        private static readonly TimeSpan GRACE_PERIOD = TimeSpan.FromSeconds(0.5);
        private const string LOCALHOST_ADDRESS = "127.0.0.1";

        /// <summary> Event is fired on a background thread. </summary>
        public event Action<T>? OnResultReceived;
        /// <summary> Event is fired on a background thread. </summary>
        public event Action? OnConnectionEstablished;
        /// <summary> Event is fired on a background thread. </summary>
        public event Action? OnConnectionLost;

        private ILogger<TrackingServerListener<T>> _logger;

        private Ping _ping;
        private int _listenerId;

        private readonly SubscriberSocket _broadcastSocket;
        private readonly string _heartbeatAddress;
        private RequestSocket _heartbeatSocket;
        private readonly MessageParser<T> _messageParser;

        private readonly NetMQPoller _poller;
        private readonly Thread _pollerThread;

        private readonly NetMQTimer _timeoutTimer;
        private bool _connectedToTrackingServer = false;

        public TrackingServerListener(int broadcastPort, int heartbeatPort, int millisecondsPerPing = 1000, int millisecondsToTimeout = 5000, ILogger<TrackingServerListener<T>>? logger = null)
        {
            _logger = logger ?? NullLogger<TrackingServerListener<T>>.Instance;

            _logger.LogDebug("Started creation of TrackingServerListener.");

            _ping = new Ping { Id = -1, IsLast = false };
            _listenerId = _ping.Id;

            _broadcastSocket = new SubscriberSocket($"tcp://{LOCALHOST_ADDRESS}:{broadcastPort}");
            _broadcastSocket.SubscribeToAnyTopic();
            _broadcastSocket.ReceiveReady += (sender, args) => ReceiveBroadcast();

            _heartbeatAddress = $"tcp://{LOCALHOST_ADDRESS}:{heartbeatPort}";
            _heartbeatSocket = new RequestSocket(_heartbeatAddress);
            _heartbeatSocket.ReceiveReady += (sender, args) => ReceivePing();

            _messageParser = new MessageParser<T>(() => new T());

            var pingSendingTimer = new NetMQTimer(millisecondsPerPing); // Order is: wait -> action -> wait -> ...
            pingSendingTimer.Elapsed += (sender, args) => SendPing();

            _timeoutTimer = new NetMQTimer(millisecondsToTimeout);
            _timeoutTimer.Elapsed += (sender, args) => Timeout();
            _timeoutTimer.Enable = false;

            SendPing();
            _logger.LogDebug("Sent initial ping.");

            _poller = new NetMQPoller { _broadcastSocket, _heartbeatSocket, pingSendingTimer, _timeoutTimer };
            _pollerThread = new Thread(_poller.Run);
            _pollerThread.Start();
            _logger.LogInformation("TrackingServerListener created.");
        }

        private void ReceiveBroadcast()
        {
            var messageBytes = _broadcastSocket.ReceiveFrameBytes();
            var message = _messageParser.ParseFrom(messageBytes);
            OnResultReceived?.Invoke(message);
        }

        private void SendPing()
        {
            _ping.Id = _listenerId;
            if (_heartbeatSocket.HasOut) // HasOut is PollOut (for REQ it is false after send until response arrive).
            {
                _heartbeatSocket.SendFrame(_ping.ToByteArray());
                _logger.LogDebug("Ping sent.");
            }
            else
            {
                _logger.LogDebug("Ping not sent.");
            }
        }

        private void ReceivePing()
        {
            _ping = Ping.Parser.ParseFrom(_heartbeatSocket.ReceiveFrameBytes());

            _logger.LogDebug("Received ping. Assigned ID: {ListenerId}", _ping.Id);

            if (_ping.Id == -1)
            {
                // Server lost track of us. Send new ping with -1 id to notify we are still present and get new id.
                _ping.IsLast = false;
                _heartbeatSocket.SendFrame(_ping.ToByteArray());
                _logger.LogWarning("Server lost track of this TrackingResultListener. Sending new connection request.");
            }

            if (!_connectedToTrackingServer)
            {
                _connectedToTrackingServer = true;
                _logger.LogInformation("Connection with Tracking server established.");
                OnConnectionEstablished?.Invoke();
            }
            _timeoutTimer.EnableAndReset();
            _listenerId = _ping.Id;
        }

        private void Timeout()
        {
            if (_connectedToTrackingServer)
            {
                _connectedToTrackingServer = false;
                _timeoutTimer.Enable = false;
                _poller.RemoveAndDispose(_heartbeatSocket);
                _heartbeatSocket = new RequestSocket(_heartbeatAddress);
                _heartbeatSocket.ReceiveReady += (sender, args) => ReceivePing();
                _poller.Add(_heartbeatSocket);
                _logger.LogWarning("Awaiting response ping from Tracking server timed out.");
                OnConnectionLost?.Invoke();
            }
        }

        private void SendLastPing()
        {
            _ping.Id = _listenerId;
            _ping.IsLast = true;
            _heartbeatSocket.SendFrame(_ping.ToByteArray());
            _logger.LogInformation("Sent disconnecting ping.");
        }

        public void Dispose()
        {
            _poller.Stop();
            _pollerThread.Join();
            _logger.LogDebug("Poller thread stopped.");

            if (_heartbeatSocket.HasOut)
            {
                // HasOut is true when server is dead or when Stop was called right in ping sending timer (ping was send but ReceiveReady wasn't
                // processed already).
                SendLastPing();
            }
            else if (_connectedToTrackingServer && _heartbeatSocket.TryReceiveFrameBytes(GRACE_PERIOD, out _))
            {
                // If connection is present and socket is busy, response was just send before closure should arrive any second. Try to wait small amount
                // of time, read response and send closing ping message. Consider server dead if no response ping arrive at the first place.
                SendLastPing();
            }

            _broadcastSocket.Dispose();
            _heartbeatSocket.Dispose();

            GC.SuppressFinalize(this);
            _logger.LogInformation("TrackingServerListener disposed.");
        }
    }
}