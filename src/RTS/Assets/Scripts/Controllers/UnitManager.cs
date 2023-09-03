using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject CursorPrefab;

    public bool IsPlayerFaction;

    #region MonoBehaviour
    private void Start()
    {
        if (IsPlayerFaction)
        {
            if (_placeUnitIndicator == null)
            {
                GameObject gameObject = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity, transform);
                _placeUnitIndicator = gameObject.GetComponent<PlaceUnitIndicator>();
            }

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
    #endregion SelectUnit

    #region PlaceUnit
    public UnitDefinition UnitToPlace { get; protected set; }

    private PlaceUnitIndicator _placeUnitIndicator;

    public void SetPlaceUnit(UnitDefinition unitDefinition)
    {
        Debug.Log("SetPlaceUnit: " + unitDefinition.Name);
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
