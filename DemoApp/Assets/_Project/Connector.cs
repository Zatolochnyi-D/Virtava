using System.Collections.Concurrent;
using UnityEngine;
using Univertracker.Client;

public class Connector : MonoBehaviour
{
    [SerializeField] private Transform _jawBone;
    [SerializeField] private float _restPosition;
    [SerializeField] private float maxRotationDeviation;
    [SerializeField] private ArkitBlendshapesAnimatable _arkitBlendshapesAnimatable;
    [SerializeField] private BoneAnimatable _boneAnimatable;

    private TrackingServerListener _tracker;
    private ConcurrentQueue<TrackingResult> _queue; // TODO: Check out Volatile and Interlock.
    private ArkitBlendshapesAnimator _animator;
    private SubprocessStarter _subprocessStarter;

    void Start()
    {
        _queue = new();
        _tracker = new TrackingServerListener();
        _tracker.OnResultReceived += ProcessResults;
        _animator = new(_boneAnimatable);
        _subprocessStarter = new();
    }

    void Update()
    {
        if (_queue.TryDequeue(out var result))
        {
            if (result.TrackingSucceded)
            {
                Debug.Log("Tracking successful");
                _animator.Apply(result);
            }
            else
            {
                Debug.Log("Tracking failed");
            }
        }
    }

    void OnDestroy()
    {
        _tracker.Dispose();
        _subprocessStarter.Dispose();
    }

    private void ProcessResults(TrackingResult result)
    {
        _queue.Enqueue(result);
    }
}