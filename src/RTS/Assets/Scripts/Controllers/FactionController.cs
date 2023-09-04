using UnityEngine;

public class FactionController : MonoBehaviour
{
    public static FactionController Instance { get; private set; }

    public FactionDefinition[] FactionDefinitions;
    public FactionDefinition CurrentFaction;

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("FactionController.OnEnable: More than one FactionController in scene.");
            gameObject.SetActive(false);
            return;
        }
        Instance = this;
    }

    public UnitManager GetPlayerUnitManager()
    {
        // TODO: Find the correct manager

        return transform.GetChild(0).GetComponent<UnitManager>();
    }
}
