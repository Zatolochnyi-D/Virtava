using UnityEngine;
using Univertracker.Client;

public class Connector2 : MonoBehaviour
{
    [SerializeField] private Transform _jawBone;
    [SerializeField] private float _restPosition;
    [SerializeField] private float maxRotationDeviation;

    private Tracker _tracker;

    void Start()
    {
        _tracker = new Tracker();
        _tracker.OnResultReceived += ProcessResults;
        _tracker.OnResultNotReceived += () => Debug.Log("Tracking failed");
    }

    void Update()
    {
        _tracker.Update();
    }

    void OnDestroy()
    {
        _tracker.Dispose();
    }

    private void ProcessResults(TrackingResult result)
    {
        Debug.Log("Tracking successful");
        var dev = maxRotationDeviation - _restPosition;
        _jawBone.localEulerAngles = _jawBone.localEulerAngles.With(z: _restPosition + dev * result.Blendshapes.JawOpen);
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