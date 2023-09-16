using RTS.Definitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RTS.UI
{
    public class PlaceUnitPanel : MonoBehaviour
    {
        public GameObject UnitButtonPrefab;

        public void CreateButtons()
        {
            transform.DestroyAllChildren();

            foreach (var unitDefinition in FactionController.Instance.CurrentFaction.UnitDefinitions)
            {
                var unitButton = Instantiate(UnitButtonPrefab);
                unitButton.transform.SetParent(transform);

                var button = unitButton.GetComponentInChildren<Button>();
                var ud = unitDefinition;
                button.onClick.AddListener(() => FactionController.Instance.GetPlayerUnitManager().SetPlaceUnit(ud));

                var buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = unitDefinition.Name;
            }
        }

        private void Start()
        {
            CreateButtons();
        }
    }
}
