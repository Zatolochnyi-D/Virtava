using UnityEngine;

public class TrackerStarter : MonoBehaviour
{
    [SerializeField] private CameraSelectionScreenController _cameraSelectionScreenController;
    [SerializeField] private ProcessManager _processManager;

    void Awake()
    {
        _cameraSelectionScreenController.OnCameraSelected += _processManager.StartTracker;
    }
}