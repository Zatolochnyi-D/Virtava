using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class ProcessManager : MonoBehaviour
{
    private const float TIME_BEFORE_RESTARTING = 0.5f;
    private const string TRACKER_APP_NAME = "mediapipe-tracking-server";

#if UNITY_STANDALONE_OSX
    private const string TRACKER_APP_LOCATION = "mediapipe-tracking-server-macos";
#elif UNITY_STANDALONE_WIN
    private const string TRACKER_APP_LOCATION = "mediapipe-tracking-server-windows";
#endif

    [SerializeField] private int _port;
    [SerializeField] private int _heartbeatPort;

    private Process _process;
    private string _selectedCamera;

    void Awake()
    {
        foreach (var process in Process.GetProcessesByName(TRACKER_APP_NAME))
            process.Kill();
    }

    private void HandleProcessExit(object sender, EventArgs e)
    {
        RestartTrackerAfterDelay();
    }

    private async void RestartTrackerAfterDelay()
    {
        await Awaitable.WaitForSecondsAsync(TIME_BEFORE_RESTARTING);
        StartTracker();
    }

    private void StartTracker()
    {
        _process = new Process
        {
            EnableRaisingEvents = true,
            StartInfo = new ProcessStartInfo()
            {
                FileName = Path.Combine(Application.streamingAssetsPath, TRACKER_APP_LOCATION, TRACKER_APP_NAME),
                Arguments = $"-p {_port} -b {_heartbeatPort} -c \"{_selectedCamera}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
            }
        };
        _process.Start();
        _process.Exited += HandleProcessExit;
    }

    public void StartTracker(string selectedCamera)
    {
        _selectedCamera = selectedCamera;
        StartTracker();
    }
}