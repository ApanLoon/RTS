using System.Collections.Generic;
using UnityEngine;

namespace RTS.UI
{
    public class UnitsPanel : MonoBehaviour
    {
        public enum PanelId
        {
            UnitSelection,
            UnitRequisition
        }

        [SerializeField] private GameObject _unitSelection;
        [SerializeField] private GameObject _unitRequisition;

        private readonly Dictionary<PanelId, GameObject> _panels = new();

        private PanelId _currentPanelId = PanelId.UnitSelection;

        public void ToggleRequisitionPanel()
        {
            SetPanel(_currentPanelId != PanelId.UnitRequisition ? PanelId.UnitRequisition : PanelId.UnitSelection);
        }

        public void SetPanel(PanelId panelId)
        {
            _panels[_currentPanelId].SetActive(false);
            _currentPanelId = panelId;
            _panels[_currentPanelId].SetActive(true);
        }

        private void Start()
        {
            _panels[PanelId.UnitSelection] = _unitSelection;
            _panels[PanelId.UnitRequisition] = _unitRequisition;
        }
    }
}

