using System.Collections.Concurrent;
using UnityEngine;
using Univertracker.Client;

public class Connector2 : MonoBehaviour
{
    [SerializeField] private Transform _jawBone;
    [SerializeField] private float _restPosition;
    [SerializeField] private float maxRotationDeviation;
    [SerializeField] private ArkitBlendshapesAnimatable _arkitBlendshapesAnimatable;

    private TrackingServerListener _tracker;
    private ConcurrentQueue<TrackingResult> _queue; // TODO: Check out Volatile and Interlock.
    private ArkitBlendshapesAnimator _animator;

    void Start()
    {
        _queue = new();
        _tracker = new TrackingServerListener();
        _tracker.OnResultReceived += ProcessResults;
        _animator = new(_arkitBlendshapesAnimatable);
    }

    void Update()
    {
        if (_queue.TryDequeue(out var result))
        {
            if (result.TrackingSucceded)
            {
                Debug.Log("Tracking successful");
                // var dev = maxRotationDeviation - _restPosition;
                // _jawBone.localEulerAngles = _jawBone.localEulerAngles.With(z: _restPosition + dev * result.Blendshapes.JawOpen);

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
    }

    private void ProcessResults(TrackingResult result)
    {
        _queue.Enqueue(result);
    }
}

public static class VectorExtension
{
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        vector.x = x ?? vector.x;
        vector.y = y ?? vector.y;
        vector.z = z ?? vector.z;
        return vector;
    }
}