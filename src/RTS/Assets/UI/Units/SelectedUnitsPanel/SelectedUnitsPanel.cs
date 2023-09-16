using System.Collections.Generic;
using UnityEngine;

namespace RTS.UI
{
    public class SelectedUnitsPanel : MonoBehaviour
    {
        public GameObject SelectedUnitPrefab;

        private Dictionary<UnitController, GameObject> _GameObjectByUnitController =
            new Dictionary<UnitController, GameObject>();

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

                var u = unit;
                unitUI.GetComponent<SelectedUnit>().SetProperties(
                    unit,
                    () => _unitManager.SelectUnits(u),
                    () => _unitManager.DeselectUnits(u)
                );
            }
        }
    }
}
