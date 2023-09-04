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

            InputController.Instance.OnCommand += OnCommand; 
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

    /// <summary>
    /// Returns to either Select or Command depending on unit selection.
    /// </summary>
    private void ReturnToIdle()
    {
        if (_selectedUnits.Count > 0)
        {
            _inputController.SetActionMap(InputController.ActionMapId.Command);
        }
        else
        {
            _inputController.SetActionMap(InputController.ActionMapId.Select);
        }
    }

    #region SelectUnit
    private bool _isSelecting;
    private Vector3 _selectStartPos;
    private List<UnitController> _selectedUnits = new List<UnitController>();

    public void SelectUnits(UnitController unitController)
    {
        _selectedUnits.Clear();
        _selectedUnits.Add(unitController);
        SelectionChanged();
    }

    public void SelectUnits(IEnumerable<UnitController> unitControllers)
    {
        _selectedUnits.Clear();
        _selectedUnits.AddRange(unitControllers);
        SelectionChanged();
    }

    public void DeselectUnits(UnitController unitController)
    {
        if (_selectedUnits.Contains(unitController))
        {
            _selectedUnits.Remove(unitController);
            SelectionChanged();
        }
    }
    public void DeselectUnits(IEnumerable<UnitController> unitControllers)
    {
        bool changed = false;
        foreach (var unitController in unitControllers)
        {
            if (_selectedUnits.Contains(unitController))
            {
                _selectedUnits.Remove(unitController);
                changed = true;
            }
        }
        if (changed)
        {
            SelectionChanged();
        }
    }

    private void SelectionChanged()
    {
        OnSelectionChanged?.Invoke(_selectedUnits);
        ReturnToIdle();
    }

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
            SelectionChanged();
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
        SelectionChanged();
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
        Vector3 pos = InputController.Instance.MouseRayHitPosition;
        var go = Instantiate (UnitToPlace.UnitPrefab, pos, Quaternion.identity);
        var unit = go.GetComponent<UnitController>();
        unit.UnitManager = this;

        if (unit.UnitDefinition.AffectsNavMesh)
        {
            TerrainManager.Instance.RebuildNavMesh(pos, unit.UnitDefinition.Size.magnitude);
        }

        // TODO: Store a reference to the unit

        _isPlacing = false;
        _groundProjector.gameObject.SetActive(false);
        UnitToPlace = null;
        ReturnToIdle();
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
        ReturnToIdle();
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

    #region Command
    private void OnCommand()
    {
        if (_inputController.HasMouseRayHit == false)
        {
            return;
        }

        var targetUnitController = _inputController.MouseRayHitObject.GetComponentInParent<UnitController>();
        if (targetUnitController != null)
        {
            // TODO: Check if the selected units can enter the target
            // TODO: Check if the target is enemy faction
            Debug.Log("UnitManager.OnCommand: Attack " + targetUnitController.gameObject.name);
            return;
        }

        // Assume the command was on the ground, issue move orders.

        Vector3?[] targetPositions = ComputeTargetPositions(_inputController.MouseRayHitPosition, _selectedUnits);

        Debug.Log("UnitManager.OnCommand: Move to " + _inputController.MouseRayHitPosition);
        int index = 0;
        foreach (var unit in _selectedUnits)
        {
            var pos = targetPositions[index++];
            if (pos != null)
            {
                unit.Command_MoveTo((Vector3)pos);
            }
        }
    }

    /// <summary>
    /// Compute target positions for the selected units
    /// 
    /// TODO: Add sophistication to this algorithm to take different unit sizes and classes into account, perhaps also allow for different formations.
    /// 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="units"></param>
    /// <returns></returns>
    private Vector3?[] ComputeTargetPositions(Vector3 center, List<UnitController> units)
    {
        List<Vector3?> l = new List<Vector3?>();
        int nCols = Mathf.FloorToInt(Mathf.Sqrt(units.Count));
        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];

            if(unit.UnitDefinition.CanMove == false)
            {
                l.Add(null);
                continue;
            }

            int row = i / nCols;
            int col = i % nCols;
            Vector3 s = unit.UnitDefinition.Size;

            // TODO: This isn't even correct. It was supposed to  place all units around the centre point but it does not.
            var offset = new Vector3((col - (nCols / 2f)) * s.x, 0f, (row - (nCols / 2f))) * s.z;
            Debug.Log($"Offset: {i} - nCols={nCols} col={col} row={row} offset={offset}");
            l.Add(center - offset);
        }
        return l.ToArray();
    }
    #endregion Command

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
