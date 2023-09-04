using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class UnitManager : MonoBehaviour
{
    public GameObject GroundProjectorPrefab;
    public Material SelectProjectorMaterial;

    public bool IsPlayerFaction;

    public event Action<List<UnitController>> OnSelectionChanged;

    private InputController _inputController;

    #region MonoBehaviour
    private void Start()
    {
        _inputController = InputController.Instance;

        if (IsPlayerFaction)
        {
            if (_groundProjector == null)
            {
                _groundProjector = Instantiate(GroundProjectorPrefab, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f), transform);
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
            Update_Select();
            Update_Place();
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
    }
    private void OnSelectEnd()
    {
        if (_isSelecting == false)
        {
            return;
        }

        _isSelecting = false;
        _groundProjector.gameObject.SetActive (false);

        if (_inputController.HasMouseRayHit == false)
        {
            Debug.Log("UnitManager.OnSelectEnd: Selection ended with no MouseRayHit. Don't know what to do so cancel the selection.");
            OnSelectionChanged?.Invoke(_selectedUnits);
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
        else
        {
            var bounds = GeometryUtility.CalculateBounds(new Vector3[] { _selectStartPos, pos }, Matrix4x4.identity);
            foreach (var collider in Physics.OverlapBox(bounds.center, new Vector3(bounds.size.x * 0.5f, 1000, bounds.size.z * 0.5f) ))
            {
                var unitController = collider.GetComponentInParent<UnitController>();
                if (unitController != null && _selectedUnits.Contains(unitController) == false)
                {
                    _selectedUnits.Add(unitController);
                }
            }
        }
        Debug.Log($"UnitManager.OnSelectEnd: selectedUnits={_selectedUnits.Count}");
        OnSelectionChanged?.Invoke(_selectedUnits);
    }

    private void Update_Select()
    {
        if (_isSelecting == false)
        {
            return;
        }

        var bounds = GeometryUtility.CalculateBounds(new Vector3[] {_selectStartPos, _inputController.MouseRayHitPosition }, Matrix4x4.identity);
        SetGroundProjector(bounds.center, SelectProjectorMaterial, bounds.size.x, bounds.size.z);
    }
    #endregion SelectUnit

    #region PlaceUnit
    public UnitDefinition UnitToPlace { get; protected set; }
    private bool _isPlacing;
    public void SetPlaceUnit(UnitDefinition unitDefinition)
    {
        Debug.Log("UnitManager.SetPlaceUnit: " + unitDefinition.Name);
        UnitToPlace = unitDefinition;
        InputController.Instance.SetActionMap(InputController.ActionMapId.Place);
        _isPlacing = true;
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

        _isPlacing = false;
        _groundProjector.gameObject.SetActive(false);
        UnitToPlace = null;
        InputController.Instance.SetActionMap(InputController.ActionMapId.Select);
    }

    private void OnCancel()
    {
        if (IsPlayerFaction == false)
        {
            return;
        }

        _isPlacing = false;
        _groundProjector.gameObject.SetActive(false);
        UnitToPlace = null;
        InputController.Instance.SetActionMap(InputController.ActionMapId.Select);
    }

    private void Update_Place()
    {
        if (_isPlacing == false)
        {
            return;
        }

        SetGroundProjector(InputController.Instance.MouseRayHitPosition, FactionController.Instance.CurrentFaction.DecalMaterial, UnitToPlace.Size.x, UnitToPlace.Size.y);
    }
    #endregion PlaceUnit

    #region GroundProjector
    private GameObject _groundProjector;

    private void SetGroundProjector(Vector3 pos, Material material, float width, float height)
    {
        _groundProjector.transform.position = pos;
        var _projector = _groundProjector.GetComponent<DecalProjector>();
        _projector.material = material;
        _projector.size = new Vector3(width, height, 50f);

        _groundProjector.gameObject.SetActive(true);
    }
    #endregion GroundProjector
}
