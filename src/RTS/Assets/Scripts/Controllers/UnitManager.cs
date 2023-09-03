using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject CursorPrefab;

    public bool IsPlayerFaction;

    public UnitDefinition UnitToPlace { get; protected set; }

    private Cursor _cursor;

    public void SetPlaceUnit(UnitDefinition unitDefinition)
    {
        Debug.Log("SetPlaceUnit: " + unitDefinition.Name);
        UnitToPlace = unitDefinition;
        InputController.Instance.SetActionMap(InputController.ActionMapId.Place);
    }


    private void Start()
    {
        if (IsPlayerFaction)
        {
            if (_cursor == null)
            {
                GameObject gameObject = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity, transform);
                _cursor = gameObject.GetComponent<Cursor>();
            }

            InputController.Instance.OnPlace              += OnPlace;
            InputController.Instance.OnCancel             += OnCancel;
        }
    }

    private void OnPlace()
    {
        if (IsPlayerFaction == false)
        {
            return;
        }

        var go = Instantiate (UnitToPlace.UnitPrefab, InputController.Instance.MouseRayHitPosition, Quaternion.identity);

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


    private void Update()
    {
        if (IsPlayerFaction)
        {
            Update_HandlePlacementProjector();
        }
    }

    private void Update_HandlePlacementProjector()
    {
        if ( UnitToPlace == null || InputController.Instance.HasMouseRayHit == false)
        {
            _cursor.gameObject.SetActive(false);
            return;
        }

        _cursor.transform.position = InputController.Instance.MouseRayHitPosition;
        _cursor.SetProjector(FactionController.Instance.CurrentFaction.DecalMaterial, UnitToPlace.Size.x, UnitToPlace.Size.y);
        _cursor.gameObject.SetActive(true);
    }
}
