using UnityEngine;
using UnityEngine.InputSystem;

public class MiniMapImageController : MonoBehaviour
{
    private MiniMapController _minimapController;
    private RectTransform _rectTransform;

    private void Start()
    {
        _minimapController = GetComponentInParent<MiniMapController>();
        _rectTransform = GetComponentInParent<RectTransform>();
    }

    public void OnClick()
    {
        var mousePos = Mouse.current.position.ReadValue();

        // Convert screen space position to range 0-1 relative to the image:
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, mousePos, null, out var localPos);
        var rect = _rectTransform.rect;
        localPos.x /= rect.width  * 0.5f;
        localPos.y /= rect.height * 0.5f;

        _minimapController.MoveCameraRigToMiniMapPosition(localPos);
    }
}
