using UnityEngine;

public class CameraRigController : MonoBehaviour
{
    public float PanSpeed = 30f;
    public float OrbitSensitivity = 0.5f;
    public bool InvertPitch = true;

    /// <summary>
    /// Min and max of the camera orbit pitch in degrees
    /// </summary>
    public Vector2 OrbitPitchLimits = new(-10, 80);

    public float DollySensitivity = 1.0f;
    /// <summary>
    /// Min and max of the camera dolly in meters
    /// </summary>
    public Vector2 DollyLimits = new(2f, 25f);


    private Camera _camera;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();

        InputController.Instance.OnCameraPan += OnCameraPan;
        InputController.Instance.OnCameraOrbit += OnCameraOrbit;
        InputController.Instance.OnCameraDolly += OnCameraDolly;
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
        transform.Translate (dir * (PanSpeed * deltaTime), Space.World);

        ClampToTerrain();
        ManageCollisions();
    }

    /// <summary>
    /// Handle camera orbit inputDelta
    /// </summary>
    /// <param name="isOrbiting">True if camera orbit is active this frame. On the frame when orbiting stops, this will be false.</param>
    /// <param name="inputDelta">Mouse position in screen coordinates</param>
    /// <param name="deltaTime">Time since last frame</param>
    private void OnCameraOrbit(bool isOrbiting, Vector2 inputDelta, float deltaTime)
    {
        if (isOrbiting == false)
        {
            return;
        }

        var delta = inputDelta * (OrbitSensitivity * deltaTime);
        if (InvertPitch)
        {
            delta.y *= -1;
        }

        transform.Rotate(new Vector3(delta.y, 0f,      0f), Space.Self);
        transform.Rotate(new Vector3(0f,      delta.x, 0f), Space.World);

        // Clamp x-rotation
        var rotEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(rotEuler.x.ClampAngle(OrbitPitchLimits.x, OrbitPitchLimits.y), rotEuler.y, rotEuler.z));

        _camera.transform.LookAt(transform, Vector3.up);

        ClampToTerrain();
        ManageCollisions();
    }

    private void OnCameraDolly(float input)
    {
        // TODO: This is probably frame rate dependent when it shouldn't
        // TODO: This should not be linear
        _camera.transform.Translate(0f, 0f, input * DollySensitivity, Space.Self);

        // Clamp local z-position
        var pos = _camera.transform.localPosition;
        var clamped = new Vector3 (pos.x, pos.y, Mathf.Clamp(pos.z, -DollyLimits.y, -DollyLimits.x));
        _camera.transform.localPosition = clamped;

        ClampToTerrain();
        ManageCollisions();
    }

    private void ManageCollisions()
    {
        // TODO: Manage camera collisions
    }

    private void ClampToTerrain()
    {
        Vector3 pos = transform.position;
        pos.y = TerrainManager.Instance.TerrainHeightAt(pos);
        transform.position = pos;
    }

    public void MoveTo(Vector3 pos)
    {
        // TODO: Smoothly move the camera rig

        transform.position = pos;
        ClampToTerrain();
    }
}
