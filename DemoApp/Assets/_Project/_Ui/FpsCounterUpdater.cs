using System.Threading;
using TMPro;
using UnityEngine;

public class FpsCounterUpdater : MonoBehaviour
{
    private const string FPS_COUNTER_FORMAT_STRING = "FPS: {0}";

    [SerializeField] private TrackingServerListenerWrapper _listener;
    [SerializeField] private TextMeshProUGUI _fpsCounterTextBar;

    private int _receiveCounter = 0;

    void Awake()
    {
        _listener.OnResultReceived += (_) => _receiveCounter++;
        FpsCounterUpdateCycle(destroyCancellationToken);
    }

    private async void FpsCounterUpdateCycle(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Awaitable.WaitForSecondsAsync(1f);
            _fpsCounterTextBar.text = string.Format(FPS_COUNTER_FORMAT_STRING, _receiveCounter);
            _receiveCounter = 0;
        }
    }
}