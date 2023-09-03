
using UnityEngine;

public static class FloatExtensions
{
    public static float ClampAngle (this float angle, float min, float max, bool inDegrees = true)
    {
        float circle = inDegrees ? 360 : 2f * Mathf.PI;

        float start = (min + max - circle) * 0.5f;
        float floor = Mathf.FloorToInt((angle - start) / circle) * circle;
        return Mathf.Clamp(angle, min + floor, max + floor);
    }
}

