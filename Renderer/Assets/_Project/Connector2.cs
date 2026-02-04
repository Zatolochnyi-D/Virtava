using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class Connector2 : MonoBehaviour
{
    private Thread _thread;
    private SubscriberSocket _socket;
    private readonly ConcurrentQueue<byte[]> _queue = new();
    private bool _running;

    void Start()
    {
        _running = true;
        _thread = new Thread(ReceiveLoop);
        // new Thread()
        _thread.Start();
    }

    void ReceiveLoop()
    {
        AsyncIO.ForceDotNet.Force(); // REQUIRED for Unity

        using (_socket = new SubscriberSocket())
        {
            _socket.Connect("tcp://localhost:13133");
            _socket.SubscribeToAnyTopic();

            while (_running)
            {
                try
                {
                    var success = _socket.TryReceiveFrameBytes(TimeSpan.FromMilliseconds(100.0), out var msg);
                    // Debug.Log($"Hey, {success}");
                    if (success)
                        _queue.Enqueue(msg);
                }
                catch
                {
                    break;
                }
            }
        }

        // NetMQConfig.Cleanup();
    }

    void Update()
    {
        while (_queue.TryDequeue(out var msg))
        {
            Debug.Log(NormalizedLandmarkPointsList.Parser.ParseFrom(msg).Points[0]);
            // Debug.Log("=====");
            // Debug.Log($"Received {msg.Length} bytes");
        }
    }

    void OnDestroy()
    {
        _running = false;
        _thread?.Join();
    }
}