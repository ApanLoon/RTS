using TMPro;
using UnityEngine;

public class InputInfoPanel : MonoBehaviour
{
    public TMP_Text ContentText;

    private InputController _inputController;

    private void Start()
    {
        _inputController = InputController.Instance;
        if (_inputController == null )
        {
            Debug.LogError("SelectionInfoPanel.Start: No InputController found.");
            gameObject.SetActive(false);
            return;
        }
    }
    private void Update()
    {
        string s = "";
        s += "ActionMap:     " + _inputController.CurrentActionMapId;

        if (_inputController.MouseRayHitPosition != null)
        {
            s += "\nMouseRayHit:   " + _inputController.MouseRayHitPosition;
        }

        if (_inputController.MouseRayHitObject != null)
        {
            s += "\nMouseRayHitObj: " + _inputController.MouseRayHitObject.name;
        }

        if (FactionController.Instance.GetPlayerUnitManager().UnitToPlace != null)
        {
            s += "\nUnitToPlace:   " + FactionController.Instance.GetPlayerUnitManager().UnitToPlace.Name;
        }

        ContentText.text = s;
    }
}
