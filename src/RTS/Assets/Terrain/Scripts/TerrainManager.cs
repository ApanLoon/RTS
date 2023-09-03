using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance { get; protected set; }

    private Terrain _terrain;

    private void OnEnable()
    {
        Instance = this;

        _terrain = GetComponentInChildren<Terrain>();
    }

    /// <summary>
    /// Returns the height, in world space, of the position above or below the given world space point.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public float TerrainHeightAt(Vector3 pos)
    {
        return _terrain.SampleHeight(pos);
    }
}
