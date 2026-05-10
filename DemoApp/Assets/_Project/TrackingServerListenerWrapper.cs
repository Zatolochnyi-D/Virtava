using System.Collections.Concurrent;
using UnityEngine;
using Virtava.Client;

public class TrackingServerListenerWrapper : MonoBehaviour
{
    private TrackingServerListener<TrackingResult> _tracker;
    private ConcurrentQueue<TrackingResult> _results;

    void Awake()
    {
        _tracker = new(14210, 14211);
        _results = new();
        _tracker.OnConnectionEstablished += () => Debug.Log("CONNECTION ESTABLISHED");
        _tracker.OnConnectionLost += () => Debug.Log("CONNECTION LOST");
        _tracker.OnResultReceived += val => _results.Enqueue(val);
    }

    void Update()
    {
        if (_results.TryDequeue(out var result))
        {
            Debug.Log(result.TrackingSucceded);
        }
    }

    void OnDestroy()
    {
        _tracker.Dispose();
    }
}