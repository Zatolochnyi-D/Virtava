using System;
using System.Collections.Concurrent;
using UnityEngine;
using Virtava.Client;

public class TrackingServerListenerWrapper : MonoBehaviour
{
    public event Action OnConnectionEstablished;
    public event Action OnConnectionLost;
    public event Action<TrackingResult> OnResultReceived;

    private TrackingServerListener<TrackingResult> _tracker;
    private ConcurrentQueue<TrackingResult> _results;
    private bool _hadConnectionEstablishedOnThisFrame = false;
    private bool _hadConnectionLostOnThisFrame = false;

    void Awake()
    {
        _tracker = new(14210, 14211);
        _results = new();
        _tracker.OnConnectionEstablished += () => _hadConnectionEstablishedOnThisFrame = true;
        _tracker.OnConnectionLost += () => _hadConnectionLostOnThisFrame = true;
        _tracker.OnResultReceived += val => _results.Enqueue(val);
    }

    void Update()
    {
        if (_results.TryDequeue(out var result))
            OnResultReceived?.Invoke(result);
        if (_hadConnectionEstablishedOnThisFrame)
        {
            OnConnectionEstablished?.Invoke();
            _hadConnectionEstablishedOnThisFrame = false;
        }
        if (_hadConnectionLostOnThisFrame)
        {
            OnConnectionLost?.Invoke();
            _hadConnectionLostOnThisFrame = false;
        }
    }

    void OnDestroy()
    {
        _tracker.Dispose();
    }
}