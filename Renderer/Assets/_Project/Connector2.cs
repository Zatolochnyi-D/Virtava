using UnityEngine;
using Univertracker.Client;

public class Connector2 : MonoBehaviour
{
    private Tracker _tracker;

    void Start()
    {
        _tracker = new Tracker();
        _tracker.OnResultReceived += (_) => Debug.Log("Tracking successful");
        _tracker.OnResultNotReceived += () => Debug.Log("Tracking failed");
    }

    void OnDestroy()
    {
        _tracker.Dispose();
    }
}