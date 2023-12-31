using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    public Transform Target;
    private Camera _camera;

    public Vector3 Position => transform.position;
    public float OrthographicSize => _camera.orthographicSize;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Target == null)
        {
            return;
        }

        transform.position = new Vector3(Target.position.x, 0f, Target.position.z);
    }
}
