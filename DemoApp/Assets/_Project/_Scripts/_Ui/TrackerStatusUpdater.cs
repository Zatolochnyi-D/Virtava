using TMPro;
using UnityEngine;
using Virtava.Adapters.Unity;
using Virtava.DataFormatModules.ArkitBlendshapes;

public class TrackerStatusUpdater : MonoBehaviour
{
    private const string TRACKER_STATUS_FORMAT_STRING = "Статус трекера: {0}";

    [SerializeField] private UnityTrackingServerListener<ArkitBlendshapesResult> _listener;
    [SerializeField] private TextMeshProUGUI _statusTextBar;

    void Awake()
    {
        _listener.OnConnectionEstablished += () => _statusTextBar.text = string.Format(TRACKER_STATUS_FORMAT_STRING, "Активний");
        _listener.OnConnectionLost += () => _statusTextBar.text = string.Format(TRACKER_STATUS_FORMAT_STRING, "Не відповідає");
    }
}