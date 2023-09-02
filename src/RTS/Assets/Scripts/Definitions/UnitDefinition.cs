using UnityEngine;

[CreateAssetMenu(fileName = "New Unit definition", menuName = "Definitions/Unit")]
public class UnitDefinition : ScriptableObject
{
    public string Name;
    public Vector3 Size;
}
