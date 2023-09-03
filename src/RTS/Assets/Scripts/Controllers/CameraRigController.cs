using UnityEngine;

public class CameraRigController : MonoBehaviour
{
    public float PanSpeed = 30f;
    public float OrbitSensitivity = 0.5f;
    public bool InvertPitch = true;

    /// <summary>
    /// Min and max of the camera orbit pitch
    /// </summary>
    public Vector2 OrbitPitchLimits = new Vector2(-10, 80);

    private Camera _camera;

    private Vector2? _prevOrbitInput = null;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();

        InputController.Instance.OnCameraPan += OnCameraPan;
        InputController.Instance.OnCameraOrbit += OnCameraOrbit;
    }

    /// <summary>
    /// Handle camera pan
    /// </summary>
    /// <param name="input"></param>
    /// <param name="deltaTime"></param>
    private void OnCameraPan(Vector2 input, float deltaTime)
    {
        var dir = transform.right * input.x;
        dir += new Vector3(transform.forward.x, 0f, transform.forward.z) * input.y;
        dir.Normalize();
        transform.Translate (dir * PanSpeed * deltaTime, Space.World);
    }

    /// <summary>
    /// Handle camera orbit input
    /// </summary>
    /// <param name="input">Mouse position in screen coordinates</param>
    /// <param name="deltaTime">Time since last frame</param>
    private void OnCameraOrbit(bool isOrbiting, Vector2 input, float deltaTime)
    {
        // TODO: How do we handle infinite orbiting when the mouse reaches the edges of the screen?

        if (isOrbiting == false)
        {
            _prevOrbitInput = null;
            return;
        }

        if (_prevOrbitInput == null)
        {
            _prevOrbitInput = input;
            return;
        }

        Vector2 delta = (input - (Vector2)_prevOrbitInput) * OrbitSensitivity * deltaTime;
        if (InvertPitch)
        {
            delta.y *= -1;
        }

        transform.Rotate(new Vector3(delta.y, 0f,      0f), Space.Self);
        transform.Rotate(new Vector3(0f,      delta.x, 0f), Space.World);

        // Clamp x-rotation
        Vector3 rotEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(rotEuler.x.ClampAngle(OrbitPitchLimits.x, OrbitPitchLimits.y), rotEuler.y, rotEuler.z));

        // TODO: Manage camera collisions

        _camera.transform.LookAt(transform, Vector3.up);

        _prevOrbitInput = input;
    }
}
