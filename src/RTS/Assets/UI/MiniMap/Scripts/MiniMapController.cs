using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [SerializeField] private CameraRigController _cameraRigController;
    [SerializeField] private MiniMapCameraController _miniMapCameraController;

    /// <summary>
    /// Moves the associated camera rig to the position defined by a position in the mini map UI element.
    /// </summary>
    /// <param name="miniMapPos">Ranges between -1 and 1</param>
    public void MoveCameraRigToMiniMapPosition(Vector2 miniMapPos)
    {
        var mmcPos = _miniMapCameraController.Position;
        var mmcSize = _miniMapCameraController.OrthographicSize;

        miniMapPos *= mmcSize;
        var pos = new Vector3 (miniMapPos.x + mmcPos.x, mmcPos.y, miniMapPos.y + mmcPos.z);

        _cameraRigController.MoveTo(pos);
    }
}
