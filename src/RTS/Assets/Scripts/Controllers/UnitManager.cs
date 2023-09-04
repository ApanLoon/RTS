using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject CursorPrefab;

    public bool IsPlayerFaction;

    private InputController _inputController;

    #region MonoBehaviour
    private void Start()
    {
        _inputController = InputController.Instance;

        if (IsPlayerFaction)
        {
            if (_placeUnitIndicator == null)
            {
                GameObject gameObject = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity, transform);
                _placeUnitIndicator = gameObject.GetComponent<DecalProjectorController>();
            }

            InputController.Instance.OnSelectStart += OnSelectStart;
            InputController.Instance.OnSelectEnd += OnSelectEnd;

            InputController.Instance.OnPlace  += OnPlace;
            InputController.Instance.OnCancel += OnCancel;
        }
    }

    private void Update()
    {
        if (IsPlayerFaction)
        {
            Update_HandlePlacementProjector();
        }
    }
    #endregion MonoBehaviour

    #region SelectUnit
    private bool _isSelecting;
    private Vector3 _selectStartPos;
    private List<UnitController> _selectedUnits = new List<UnitController>();
    private void OnSelectStart()
    {
        if (_inputController.HasMouseRayHit == false)
        {
            return;
        }
        _isSelecting = true;
        _selectStartPos = _inputController.MouseRayHitPosition;
        _selectedUnits.Clear();
        Debug.Log($"UnitManager.OnSelectStart: pos={_selectStartPos}");
    }
    private void OnSelectEnd()
    {
        if (_isSelecting == false)
        {
            return;
        }

        _isSelecting = false;

        if (_inputController.HasMouseRayHit == false)
        {
            Debug.Log("UnitManager.OnSelectEnd: Selection ended with no MouseRayHit. Don't know what to do so cancel the selection.");
            return;
        }

        Vector3 pos = _inputController.MouseRayHitPosition;

        if ((pos - _selectStartPos).sqrMagnitude < 0.1f) // TODO: Threshold should be a parameter?
        {
            // This was a single selection
            if (_inputController.MouseRayHitObject != null)
            {
                var unitController = _inputController.MouseRayHitObject.GetComponentInParent<UnitController>();
                if (unitController != null)
                {
                    _selectedUnits.Add(unitController);
                }
            }
        }
        Debug.Log($"UnitManager.OnSelectEnd: pos={pos} selectedUnits={_selectedUnits.Count}");

    }
    #endregion SelectUnit

    #region PlaceUnit
    public UnitDefinition UnitToPlace { get; protected set; }

    private DecalProjectorController _placeUnitIndicator;

    public void SetPlaceUnit(UnitDefinition unitDefinition)
    {
        Debug.Log("UnitManager.SetPlaceUnit: " + unitDefinition.Name);
        UnitToPlace = unitDefinition;
        InputController.Instance.SetActionMap(InputController.ActionMapId.Place);
    }
    
    private void OnPlace()
    {
        if (IsPlayerFaction == false)
        {
            return;
        }

        var go = Instantiate (UnitToPlace.UnitPrefab, InputController.Instance.MouseRayHitPosition, Quaternion.identity);
        var unit = go.GetComponent<UnitController>();
        unit.UnitManager = this;

        // TODO: Store a reference to the unit

        UnitToPlace = null;
        InputController.Instance.SetActionMap(InputController.ActionMapId.Select);
    }

    private void OnCancel()
    {
        if (IsPlayerFaction == false)
        {
            return;
        }

        UnitToPlace = null;
        InputController.Instance.SetActionMap(InputController.ActionMapId.Select);
    }

    private void Update_HandlePlacementProjector()
    {
        if ( UnitToPlace == null || InputController.Instance.HasMouseRayHit == false)
        {
            _placeUnitIndicator.gameObject.SetActive(false);
            return;
        }

        _placeUnitIndicator.transform.position = InputController.Instance.MouseRayHitPosition;
        _placeUnitIndicator.SetProjector(FactionController.Instance.CurrentFaction.DecalMaterial, UnitToPlace.Size.x, UnitToPlace.Size.y);
        _placeUnitIndicator.gameObject.SetActive(true);
    }
    #endregion PlaceUnit
}
