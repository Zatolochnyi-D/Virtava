using System;
using System.Collections.Concurrent;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace Univertracker.Client
{
    public class Tracker : IDisposable
    {
        public event Action<TrackingResult> OnResultReceived = null!;
        public event Action OnResultNotReceived = null!;

        private Thread _thread;
        private readonly ConcurrentQueue<byte[]> _queue = new ConcurrentQueue<byte[]>();
        private bool _running;

        public Tracker()
        {
            _running = true;
            _thread = new Thread(ReceiveLoop);
            _thread.Start();
        }

        private void ReceiveLoop()
        {
            using SubscriberSocket socket = new SubscriberSocket();

            socket.Connect("tcp://localhost:13133");
            socket.SubscribeToAnyTopic();

            while (_running)
            {
                try
                {
                    var success = socket.TryReceiveFrameBytes(TimeSpan.FromMilliseconds(100.0), out var msg);
                    if (success)
                    {
                        _queue.Enqueue(msg!);
                    }
                }
                catch //(Exception e)
                {
                    // Debug.Log(e.GetType());
                    // Debug.Log(e.Message);
                    break;
                }
            }
        }

        public void Update()
        {
            while (_queue.TryDequeue(out var msg))
            {
                var result = TrackingResult.Parser.ParseFrom(msg);
                if (result.TrackingSucceded)
                {
                    OnResultReceived?.Invoke(result);
                }
                else
                {
                    OnResultNotReceived?.Invoke();
                }
            }
        }

        public void Dispose()
        {
            _running = false;
            _thread?.Join();
        }
    }
}
