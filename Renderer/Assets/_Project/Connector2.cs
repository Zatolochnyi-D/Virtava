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
        _thread.Start();
    }

    void ReceiveLoop()
    {
        AsyncIO.ForceDotNet.Force();

        using (_socket = new SubscriberSocket())
        {
            _socket.Connect("tcp://localhost:13133");
            _socket.SubscribeToAnyTopic();

            while (_running)
            {
                try
                {
                    var success = _socket.TryReceiveFrameBytes(TimeSpan.FromMilliseconds(100.0), out var msg);
                    if (success)
                        _queue.Enqueue(msg);
                }
                catch (Exception e)
                {
                    Debug.Log(e.GetType());
                    Debug.Log(e.Message);
                    break;
                }
            }
        }
    }

    void Update()
    {
        while (_queue.TryDequeue(out var msg))
        {
            var result = TrackingResult.Parser.ParseFrom(msg);
            if (result.TrackingSucceded)
            {
                Debug.Log("Tracking successful");
                Debug.Log(result.Blendshapes.BrowDownLeft);
            }
            else
            {
                Debug.Log("Tracking failed");
            }
        }
    }

    void OnDestroy()
    {
        _running = false;
        _thread?.Join();
    }
}