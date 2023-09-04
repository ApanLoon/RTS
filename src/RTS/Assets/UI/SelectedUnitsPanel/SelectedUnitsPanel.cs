using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedUnitsPanel : MonoBehaviour
{
    public GameObject SelectedUnitPrefab;

    private Dictionary<UnitController, GameObject> _GameObjectByUnitController = new Dictionary<UnitController, GameObject>();
    private UnitManager _unitManager;

    private void Start()
    {
        _unitManager = FactionController.Instance.GetPlayerUnitManager();

        _unitManager.OnSelectionChanged += OnSelectionChanged;
    }

    public void OnSelectionChanged(List<UnitController> selectedUnits)
    {
        transform.DestroyAllChildren();

        foreach (UnitController unit in selectedUnits)
        {
            var unitUI = Instantiate(SelectedUnitPrefab);
            unitUI.transform.SetParent(transform);

        //    var button = unitButton.GetComponentInChildren<Button>();
        //    var ud = unitDefinition;
        //    button.onClick.AddListener(() => FactionController.Instance.GetPlayerUnitManager().SetPlaceUnit(ud));

            var title = unitUI.GetComponentInChildren<TMP_Text>();
            title.text = unit.gameObject.name;
        }
    }
}
