using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PlaceUnitIndicator : MonoBehaviour
{
    private DecalProjector _projector;

    void Start()
    {
        _projector = GetComponentInChildren<DecalProjector>();    
    }

    public void SetProjector(Material material, float width, float height)
    {
        _projector.material = material;
        _projector.size = new Vector3(width, height, 50);
    }
}
