using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController Instance;

    public GameObject CursorPrefab;
    public Camera CurrentCamera;
 
    public Vector3? MouseHitPosition;
    public GameObject MouseHitObject;

    public enum InputState
    {
        Select,
        Place
    }
    public InputState State { get; protected set; }

    private PlayerInput _playerInput;
    private Cursor _cursor;
    private Vector2 _mousePos;

    public UnitDefinition UnitToPlace {  get; protected set; }


    private string[] _actionMapNames =     // Must match InputState
    {
        "Select",
        "Place"
    };

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("InputController.OnEnable: There is more than one instance");
            gameObject.SetActive(false);
            return;
        }
        Instance = this;

        if (_cursor == null)
        {
            GameObject gameObject = Instantiate(CursorPrefab, Vector3.zero, Quaternion.identity, transform);
            _cursor = gameObject.GetComponent<Cursor>();
        }
    }

    private void Start()
    {
        if (CurrentCamera == null)
        {
            CurrentCamera = Camera.main;
        }
        if (CurrentCamera == null)
        {
            Debug.LogError("InputController.Start: No Camera found.");
            gameObject.SetActive(false);
            return;
        }

        _playerInput = GetComponent<PlayerInput>();

        SetState(InputState.Select);
    }

    private void Update()
    {
        switch (State)
        {
            case InputState.Select:
                Update_Select();
                break;
            case InputState.Place:
                Update_Place();
                break;
            default:
                break;
        }
    }

    private void Update_Select()
    {

    }

    private void Update_Place()
    {
        if (UnitToPlace == null)
        {
            return;
        }

        MouseHitPosition = null;
        MouseHitObject = null;
        if (Physics.Raycast(CurrentCamera.ScreenPointToRay(_mousePos), out RaycastHit hit))
        {
            MouseHitPosition = hit.point;
            MouseHitObject = hit.collider.gameObject;

            _cursor.transform.position = hit.point;
            _cursor.SetProjector(FactionController.Instance.CurrentFaction.DecalMaterial, UnitToPlace.Size.x, UnitToPlace.Size.y);
            _cursor.gameObject.SetActive(true);
        }
        else
        {
            _cursor.gameObject.SetActive(false);
        }
    }

    private void OnMousePos(InputValue inputValue)
    {
        _mousePos = inputValue.Get<Vector2>();
    }

    private void OnSelect()
    {
        Debug.Log("OnSelect");
    }

    private void OnPlace()
    {
        Debug.Log("OnPlace");
        SetState(InputState.Select);
    }

    private void OnCancel()
    {
        Debug.Log("OnCancel");
        SetState(InputState.Select);
    }

    private void SetState(InputState state)
    {
        // Leave state:
        switch (State)
        {
            case InputState.Select:
                break;
            case InputState.Place:
                _cursor.gameObject.SetActive(false);
                break;
            default:
                break;
        }

        // Enter state:
        State = state;
        _playerInput.currentActionMap = _playerInput.actions.FindActionMap(_actionMapNames[(int)State]);

    }

    public void SetPlaceUnit(UnitDefinition unitDefinition)
    {
        Debug.Log("SetPlaceUnit: " + unitDefinition.Name);
        UnitToPlace = unitDefinition;
        SetState(InputState.Place);
    }
}
