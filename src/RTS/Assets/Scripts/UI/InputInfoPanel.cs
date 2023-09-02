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
        s += "State:    " + _inputController.State;

        switch (_inputController.State)
        {
            case InputController.InputState.Select:
                break;
            case InputController.InputState.Place:
                if (_inputController.MouseHitPosition != null)
                {
                    s += "\nPos:    " + _inputController.MouseHitPosition;
                }

                s += "\nUnit:   " + _inputController.UnitToPlace.Name;

                if (_inputController.MouseHitObject != null)
                {
                    s += "\nObject: " + _inputController.MouseHitObject.name;
                }
                break;
            default:
                break;
        }

        ContentText.text = s;
    }
}
