using RTS.Definitions;
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

    private void Update()
    {
        #region Debug
        for (var i = 0; i < transform.childCount; i++)
        {
            var unitManager = transform.GetChild(i).GetComponent<UnitManager>();
            if (unitManager.UnitToPlace == null)
            {
                DebugInfoPanel.Remove($"{unitManager.FactionDefinition.Name} UnitToPlace");
            }
            else
            {
                DebugInfoPanel.Log($"{unitManager.FactionDefinition.Name} UnitToPlace", unitManager.UnitToPlace.Name);
            }
        }
        #endregion Debug
    }

    public UnitManager GetPlayerUnitManager()
    {
        // TODO: Find the correct manager

        return transform.GetChild(0).GetComponent<UnitManager>();
    }
}
