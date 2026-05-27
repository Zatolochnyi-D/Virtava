using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraSelectionScreenController : MonoBehaviour
{
    public event Action<string> OnCameraSelected;

    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private CameraPreviewController _cameraPreview;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private NoCamerasScreenController _noCamerasScreenController;
    [SerializeField] private GameObject _demoScreen;

    private List<string> _availableCameras;
    private List<WebCamTexture> _cameras;
    private int _selectedIndex;

    void Awake()
    {
        _demoScreen.SetActive(false);
        _availableCameras = WebCamTexture.devices.Select(x => x.name).ToList();
        if (!_availableCameras.Any())
        {
            _noCamerasScreenController.Activate();
            return;
        }
        _cameras = _availableCameras.Select(x => new WebCamTexture(x)).ToList();
        _selectedIndex = 0;
        _cameraPreview.Hide();
        PrepareSelection();

        _dropdown.ClearOptions();
        _dropdown.AddOptions(_availableCameras.ToList());
        _dropdown.onValueChanged.AddListener(HandleDropboxSelection);
        _dropdown.interactable = false;

        _confirmButton.onClick.AddListener(() =>
        {
            _cameras.ForEach(x => x.Stop());
            _cameras.ForEach(x => Destroy(x));
            _cameras.Clear();
            _cameraPreview.SetTexture(null);
            gameObject.SetActive(false);
            _demoScreen.SetActive(true);
            OnCameraSelected?.Invoke(_availableCameras[_selectedIndex]);
        });
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
        _cameras.ForEach(x => x.Pause());
        _dropdown.interactable = true;
        _cameraPreview.Show();
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
        _cameraPreview.SetAspectRatio((float)_cameras[_selectedIndex].width / _cameras[_selectedIndex].height);
        _cameraPreview.SetTexture(_cameras[_selectedIndex]);
        _cameras[_selectedIndex].Play();
    }
}