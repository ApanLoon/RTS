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
    /// The center and radius parameters will be used to identify which NavMeshSurfaces that has to be re-built.
    /// </summary>
    /// <param name="center">Center point of the change</param>
    /// <param name="radius">Radius of the change</param>
    public void RebuildNavMesh(Vector3 center, float radius)
    {
        // TODO: This is prohibitively expensive. Break up the nav mesh surfaces into smaller chunks and only update the chunks that were actually affected. (Quad-tree of NavMeshSurface for quick lookup?)
        _navMeshSurface.BuildNavMesh();
    }
}
