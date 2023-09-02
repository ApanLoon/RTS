
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction definition", menuName = "Definitions/Faction")]
public class FactionDefinition : ScriptableObject
{
    public string Name;
    public Color Colour;
    public Material DecalMaterial;

    public List<UnitDefinition> UnitDefinitions;
}
