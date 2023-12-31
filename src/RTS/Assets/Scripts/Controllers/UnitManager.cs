using System;
using System.Collections.Generic;
using System.Linq;
using RTS.Definitions;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class UnitManager : MonoBehaviour
{
    public GameObject GroundProjectorPrefab;
    public Material SelectProjectorMaterial;

    public FactionDefinition FactionDefinition;
    public bool IsPlayerFaction;

    public float SelectedAudioClipCoolDown = 1f;

    public event Action<List<UnitController>> OnSelectionChanged;

    private InputController _inputController;

    private void PlaceDummyUnits()
    {
        switch (FactionDefinition.Name)
        {
            case "Galactic Empire":
                PlaceUnit(FactionDefinition.UnitDefinitions[0], new Vector3(-31f, 50f, -28f), "Alpha");

                PlaceUnit(FactionDefinition.UnitDefinitions[2], new Vector3(-32f, 61f, -42f), "TK 9362");

                PlaceUnit(FactionDefinition.UnitDefinitions[2], new Vector3(-25f, 50f, -45f), "TK 1402");
                PlaceUnit(FactionDefinition.UnitDefinitions[2], new Vector3(-27f, 50f, -45f), "TK 1138");
                PlaceUnit(FactionDefinition.UnitDefinitions[2], new Vector3(-29f, 50f, -45f), "TK 1654");
                PlaceUnit(FactionDefinition.UnitDefinitions[2], new Vector3(-31f, 50f, -45f), "TK 1313");
                PlaceUnit(FactionDefinition.UnitDefinitions[2], new Vector3(-33f, 50f, -45f), "TK 3182");
                PlaceUnit(FactionDefinition.UnitDefinitions[2], new Vector3(-35f, 50f, -45f), "TK 4098");
                break;

            case "Rebel Alliance":
                PlaceUnit(FactionDefinition.UnitDefinitions[0], new Vector3(-2f, 63f, -28f), "Vero Vene");
                PlaceUnit(FactionDefinition.UnitDefinitions[0], new Vector3(-4f, 63f, -28f), "Gavarl Dystra");
                PlaceUnit(FactionDefinition.UnitDefinitions[0], new Vector3(-6f, 63f, -28f), "Val Horne");
                PlaceUnit(FactionDefinition.UnitDefinitions[0], new Vector3(-8f, 63f, -28f), "Maxir Thule");
                break;
        }
    }

    #region MonoBehaviour
    private void Start()
    {
        _inputController = InputController.Instance;

        if (IsPlayerFaction)
        {
            InputController.Instance.OnSelectStart += OnSelectStart;
            InputController.Instance.OnSelectEnd += OnSelectEnd;

            InputController.Instance.OnPlace  += OnPlace;
            InputController.Instance.OnCancel += OnCancel;

            InputController.Instance.OnCommand += OnCommand;
        }

        PlaceDummyUnits();
    }

    private void Update()
    {
        _selectedAudioClipCoolDownRemaining = Mathf.Max(_selectedAudioClipCoolDownRemaining - Time.deltaTime, 0f);

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
    private readonly List<UnitController> _selectedUnits = new();
    private float _selectedAudioClipCoolDownRemaining;

    public void SelectUnits(UnitController unitController)
    {
        _selectedUnits.Clear();
        _selectedUnits.Add(unitController);

        ElectLeader();

        SelectionChanged();
    }

    public void SelectUnits(IEnumerable<UnitController> unitControllers)
    {
        _selectedUnits.Clear();
        _selectedUnits.AddRange(unitControllers);

        ElectLeader();

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

    private void OnSelectStart(bool add)
    {
        if (_inputController.HasMouseRayHit == false)
        {
            return;
        }
        _isSelecting = true;
        _selectStartPos = _inputController.MouseRayHitPosition;

        if (add == false)
        {
            _selectedUnits.Clear();
        }
    }
    private void OnSelectEnd(bool add)
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

        var pos = _inputController.MouseRayHitPosition;

        if ((pos - _selectStartPos).sqrMagnitude < 0.1f) // TODO: Threshold should be a parameter?
        {
            // This was a single selection
            if (_inputController.MouseRayHitObject != null)
            {
                var unitController = _inputController.MouseRayHitObject.GetComponentInParent<UnitController>();
                AddToSelection(unitController, add);
            }
        }
        else
        {
            var bounds = GeometryUtility.CalculateBounds(new Vector3[] { _selectStartPos, pos }, Matrix4x4.identity);

            foreach (var unitController in Physics.OverlapBox(
                            bounds.center, 
                            new Vector3(bounds.size.x * 0.5f, 1000, bounds.size.z * 0.5f)
                         ).Select(x => x.GetComponentInParent<UnitController>()).Where(x=> x != null).Distinct()
                    )
            {
                AddToSelection(unitController, add);
            }
        }
        SelectionChanged();
    }

    private void AddToSelection(UnitController unitController, bool add)
    {
        if (unitController == null)
        {
            return;
        }
        if (_selectedUnits.Contains(unitController))
        {
            if (add)
            {
                _selectedUnits.Remove(unitController);
            }
        }
        else
        {
            _selectedUnits.Add(unitController);
        }

        ElectLeader();
    }

    private void Update_Select()
    {
        if (_isSelecting == false)
        {
            return;
        }

        var bounds = GeometryUtility.CalculateBounds(new[] {_selectStartPos, _inputController.MouseRayHitPosition }, Matrix4x4.identity);
        SetGroundProjector(bounds.center, SelectProjectorMaterial, bounds.size.x, bounds.size.z);
    }

    private void ElectLeader()
    {
        // TODO: Elect group leader

        if (_selectedAudioClipCoolDownRemaining <= 0)
        {
            var leader = _selectedUnits[0];
            AudioController.PlayAudioClip(leader.UnitDefinition.GetRandomSelectedAudioClip());
            _selectedAudioClipCoolDownRemaining = SelectedAudioClipCoolDown;
        }
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

    private void PlaceUnit(UnitDefinition unitDefinition, Vector3 pos, string designation)
    {
        pos.y = TerrainManager.Instance.TerrainHeightAt(pos);
        Debug.Log($"Placing {unitDefinition.Name} {designation} at {pos}.");

        var go = Instantiate(unitDefinition.UnitPrefab, pos, Quaternion.identity, transform);
        go.name = $"{unitDefinition.Name} {designation}";
        var unit = go.GetComponent<UnitController>();
        unit.UnitManager = this;

        if (unit.UnitDefinition.AffectsNavMesh)
        {
            TerrainManager.Instance.UpdateNavMesh();
        }

        // TODO: Store a reference to the unit
    }

    private void OnPlace()
    {
        if (IsPlayerFaction == false)
        {
            return;
        }
        var pos = InputController.Instance.MouseRayHitPosition;
        
        PlaceUnit(UnitToPlace, pos, "");

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

        var units = _selectedUnits.Where(x => x.UnitDefinition.CanMove).ToList();
        var targetPositions = ComputeTargetPositions(_inputController.MouseRayHitPosition, units);

        //Debug.Log("UnitManager.OnCommand: Move to " + _inputController.MouseRayHitPosition);
        int index = 0;
        foreach (var unit in units)
        {
            var pos = targetPositions[index++];
            if (pos != null)
            {
                unit.Command_MoveTo((Vector3)pos);
            }
        }


        // TODO: Get group leader
        var leader = _selectedUnits[0];
        AudioController.PlayAudioClip(leader.UnitDefinition.GetRandomMoveToAudioClip());
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
        var l = new List<Vector3?>();
        var nCols = Mathf.FloorToInt(Mathf.Sqrt(units.Count));
        for (var i = 0; i < units.Count; i++)
        {
            var unit = units[i];

            if(unit.UnitDefinition.CanMove == false)
            {
                l.Add(null);
                continue;
            }

            var row = i / nCols;
            var col = i % nCols;
            var s = unit.UnitDefinition.Size;

            // TODO: This isn't quite correct. It was supposed to  place all units around the centre point but it does not.
            var offset = new Vector3((col - (nCols / 2f)) * s.x, 0f, (row - (nCols / 2f))) * s.z;
            //Debug.Log($"Offset: {i} - nCols={nCols} col={col} row={row} offset={offset}");
            l.Add(center - offset);
        }
        return l.ToArray();
    }
    #endregion Command

    #region GroundProjector
    private GameObject _groundProjector;

    private void SetGroundProjector(Vector3 pos, Material material, float width, float height)
    {
        if (_groundProjector == null)
        {
            _groundProjector = Instantiate(GroundProjectorPrefab, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f), transform);
        }

        _groundProjector.transform.position = pos;
        var projector = _groundProjector.GetComponent<DecalProjector>();
        projector.material = material;
        projector.size = new Vector3(width, height, 50f);

        _groundProjector.gameObject.SetActive(true);
    }
    #endregion GroundProjector
}
