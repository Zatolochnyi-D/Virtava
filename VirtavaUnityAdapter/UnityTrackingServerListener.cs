using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;
using Virtava.Client;

namespace Virtava.Adapters.Unity
{
    public abstract class UnityTrackingServerListener<T> : MonoBehaviour where T : IMessage<T>, new()
    {
        public event Action OnConnectionEstablished;
        public event Action OnConnectionLost;
        public event Action<T> OnResultReceived;

        [SerializeField] private int _port;
        [SerializeField] private int _heartbeatPort;

        private TrackingServerListener<T> _tracker;
        private bool _isInitialized = false;
        private ConcurrentQueue<T> _results;
        private bool _hadConnectionEstablishedOnThisFrame = false;
        private bool _hadConnectionLostOnThisFrame = false;
        private bool _applicationIsFocused = true;

        void Awake()
        {
            StartAfterDelay();
        }

        void OnApplicationFocus(bool focus)
        {
            // When app is unfocused, listener thread is working whereas main thread (with Updates) halts.
            // This leads to messages enqueing and rapidly dequeing on focusing window, causing sudden 100+ FPS for the first second.
            // Let the thread work but stop enquing if window is unfocused.
            _applicationIsFocused = focus;
        }

        void Update()
        {
            if (!_isInitialized)
                return;
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

        private async void StartAfterDelay()
        {
            await Task.Delay(500);
            _tracker = new(_port, _heartbeatPort);
            _results = new();
            _tracker.OnConnectionEstablished += HandleOnConnectionEstablished;
            _tracker.OnConnectionLost += HandleOnConnectionLost;
            _tracker.OnResultReceived += HandleOnResultsReceived;
            _isInitialized = true;
        }

        private void HandleOnConnectionEstablished()
        {
            _hadConnectionEstablishedOnThisFrame = true;
        }

        private void HandleOnConnectionLost()
        {
            _hadConnectionLostOnThisFrame = true;
        }

        private void HandleOnResultsReceived(T results)
        {
            if (_applicationIsFocused)
                _results.Enqueue(results);
        }
    }
}