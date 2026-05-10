using UnityEngine;

public class LogicComponent : MonoBehaviour
{
    [SerializeField] private TrackingServerListenerWrapper _wrapper;
    [SerializeField] private Animatable _animatable;

    void Awake()
    {
        _wrapper.OnResultReceived += HandleResults;
    }

    private void HandleResults(TrackingResult result)
    {
        
        Debug.Log(result.TrackingSucceded);
    }
}