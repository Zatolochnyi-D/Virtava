using UnityEngine;
using UnityEngine.UI;

public class CameraPreviewController : MonoBehaviour
{
    [SerializeField] private RawImage _cameraOutput;
    [SerializeField] private RectTransform _cameraOutputTransform;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetAspectRatio(float aspectRatio)
    {
        _aspectRatioFitter.aspectRatio = aspectRatio;
    }

    public void SetTexture(WebCamTexture texture)
    {
        _cameraOutput.texture = texture;
    }
}