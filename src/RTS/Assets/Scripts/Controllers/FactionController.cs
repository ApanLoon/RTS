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
}
