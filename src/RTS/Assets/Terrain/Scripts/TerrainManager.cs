using Unity.AI.Navigation;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance { get; protected set; }

    private Terrain _terrain;
    private NavMeshSurface _navMeshSurface;

    private void OnEnable()
    {
        Instance = this;

        _terrain = GetComponentInChildren<Terrain>();
        _navMeshSurface = GetComponentInChildren<NavMeshSurface>();
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

    /// <summary>
    /// Call this when a fixed unit has been placed or removed.
    ///
    /// TODO: Agents who are currently moving glitch a bit when this is called.
    /// </summary>
    public void UpdateNavMesh()
    {
        _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
    }
}
