using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Start()
    {
        InputController.Instance.OnCameraOrbit += OnCameraOrbit;
    }

    private void OnCameraOrbit(bool isOrbiting, Vector2 delta, float deltaTime)
    {
        Cursor.visible = isOrbiting == false;
    }
}

