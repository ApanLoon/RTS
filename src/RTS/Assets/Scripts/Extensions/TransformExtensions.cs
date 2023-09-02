using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyAllChildren (this Transform transform)
    {
        while (transform.childCount > 0)
        {
            Transform t = transform.GetChild(0);
            t.SetParent(null); // Become Batman
            GameObject.Destroy(t.gameObject);
        }
    }
}
