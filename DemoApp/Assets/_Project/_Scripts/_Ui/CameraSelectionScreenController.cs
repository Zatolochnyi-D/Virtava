using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Create UI before viewport with available cameras dropdown, camera preview and proceed button.
// On proceed app should start tracker worker. Pass selected camera to it as an argument.
// Add clargs read to tracker program. Add --camera parameter or something to read and use in camera retrieval.
// It is important to build strong foundation from the beggining for this clargs system, cause I will use it later to pass the ports and other stuff.

public class CameraSelectionScreenController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private RawImage _cameraOutput;
    private RectTransform _cameraOutputTransform;

    private IEnumerable<string> _availableCameras;
    private List<WebCamTexture> _cameras;
    private int _selectedIndex;

    void Awake()
    {
        _cameraOutputTransform = _cameraOutput.rectTransform;

        _availableCameras = WebCamTexture.devices.Select(x => x.name);
        _cameras = _availableCameras.Select(x => new WebCamTexture(x)).ToList();
        _selectedIndex = 0;
        PrepareSelection();

        _dropdown.ClearOptions();
        _dropdown.AddOptions(_availableCameras.ToList());
        _dropdown.onValueChanged.AddListener(HandleDropboxSelection);
        _dropdown.interactable = false;
    }

    void OnDestroy()
    {
        _cameras.ForEach(x => x.Stop());
    }

    private async void PrepareSelection()
    {
        _cameras.ForEach(x => x.Play());
        while (_cameras.Any(x => !x.didUpdateThisFrame))
            await Awaitable.NextFrameAsync();
        Debug.Log("Cameras initialized.");
        _cameras.ForEach(x => x.Pause());
        _dropdown.interactable = true;
        StartCamera();
    }

    private void HandleDropboxSelection(int value)
    {
        _selectedIndex = value;
        StartCamera();
    }

    private void StartCamera()
    {
        _cameras.ForEach(x => x.Pause());
        _cameraOutputTransform.sizeDelta = new(_cameras[_selectedIndex].width, _cameras[_selectedIndex].height);
        _cameraOutput.texture = _cameras[_selectedIndex];
        _cameras[_selectedIndex].Play();
    }
}

public static class LinqExtension
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var el in collection)
            action.Invoke(el);
    }
}