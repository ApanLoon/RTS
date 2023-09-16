using UnityEngine;
namespace RTS.Definitions
{
    [CreateAssetMenu(fileName = "New Unit definition", menuName = "Definitions/Unit")]
    public class UnitDefinition : ScriptableObject
    {
    /// <summary>
    /// Name of the unit
    /// </summary>
    public string Name;

    /// <summary>
    /// Size of the unit bounding box, used for sizing the "place" projection among other things.
    /// </summary>
    public Vector3 Size;

    /// <summary>
    /// The in-world prefab
    /// </summary>
    public GameObject UnitPrefab;

    /// <summary>
    /// If this unit is a stationary building, we might have to re-build the nav mesh when it is placed or removed
    /// </summary>
    public bool AffectsNavMesh = false;

    /// <summary>
    /// If this is false, any movement commands sent to units of this type will be ignored
    /// </summary>
    public bool CanMove = false;

    /// <summary>
    /// Not all units have deflector shields
    /// </summary>
    public bool HasDeflector = false;
    }
}