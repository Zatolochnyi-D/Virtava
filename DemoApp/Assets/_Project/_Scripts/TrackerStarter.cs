using UnityEngine;

public class TrackerStarter : MonoBehaviour
{
    [SerializeField] private CameraSelectionScreenController _cameraSelectionScreenController;

    void Awake()
    {
        _cameraSelectionScreenController.OnCameraSelected += (val) => Debug.Log(val);
    }
}