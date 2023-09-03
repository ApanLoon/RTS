using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController Instance;

    public GameObject CursorPrefab;
    public Camera CurrentCamera;
    public UnitDefinition UnitToPlace { get; protected set; }

    #region events
    public event Action<Vector2, float> OnCameraPan;
    public event Action<bool, Vector2, float> OnCameraOrbit;
    #endregion events

    /// <summary>
    /// True if the raycast into the scene hit something this frame.
    /// </summary>
    public bool HasMouseHit { get; protected set; }

    /// <summary>
    /// The world position where the raycast into the scene hit something. Only use if HasMouseHit is true.
    /// </summary>
    public Vector3 MouseHitPosition { get; protected set; }

    /// <summary>
    /// The GameObject the raycast into the scene hit. Only use if HasMouseHit is true.
    /// </summary>
    public GameObject MouseHitObject { get; protected set; }

    public enum InputState
    {
        Select,
        Place
    }
    public InputState State { get; protected set; }

    private PlayerInput _playerInput;
    private Cursor _cursor;
    private Vector2 _mousePos;
    private Vector2 _cameraMove;
    private bool _onCameraOrbitTrigger;
    private bool _prevOnCameraOrbitTrigger;

    private class ActionTrigger
    {
        public bool IsTriggered;
        public Action Action;
    }
    private enum ActionTriggerId { OnSelect, OnPlace, OnCancel };
    private ActionTrigger[] _actionTriggers;



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

        if (_actionTriggers == null)
        {
            _actionTriggers = new ActionTrigger[]
            {
                new ActionTrigger() { IsTriggered = false, Action = DoSelect },
                new ActionTrigger() { IsTriggered = false, Action = DoPlace  },
                new ActionTrigger() { IsTriggered = false, Action = DoCancel },
             };
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
        float deltaTime = Time.deltaTime; // TODO: Add pause/speed control here

        foreach (var actionTrigger in _actionTriggers)
        {
            if (actionTrigger.IsTriggered)
            {
                actionTrigger.IsTriggered = false;
                if (EventSystem.current.IsPointerOverGameObject() == false) // TODO: This is an ugly way to do it, what if the action is not triggered by a click or anything else that should be eaten by the UI? For example OnCancel whitch triggers on the escape key.
                {
                    actionTrigger.Action();
                }
            }
        }

        switch (State)
        {
            case InputState.Select:
                Update_Select(deltaTime);
                Update_Camera(deltaTime);
                break;
            case InputState.Place:
                Update_Place(deltaTime);
                Update_Camera(deltaTime);
                break;
            default:
                break;
        }
    }

    private void Update_Select(float deltaTime)
    {

    }

    private void Update_Place(float deltaTime)
    {
        if (UnitToPlace == null)
        {
            return;
        }

        HasMouseHit = false;
        MouseHitObject = null;
        if (Physics.Raycast(CurrentCamera.ScreenPointToRay(_mousePos), out RaycastHit hit))
        {
            MouseHitPosition = hit.point;
            MouseHitObject = hit.collider.gameObject;
            HasMouseHit = true;

            _cursor.transform.position = hit.point;
            _cursor.SetProjector(FactionController.Instance.CurrentFaction.DecalMaterial, UnitToPlace.Size.x, UnitToPlace.Size.y);
            _cursor.gameObject.SetActive(true);
        }
        else
        {
            HasMouseHit = false;
            _cursor.gameObject.SetActive(false);
        }
    }

    private void Update_Camera(float deltaTime)
    {
        if (_cameraMove.sqrMagnitude > 0f)
        {
            OnCameraPan?.Invoke(_cameraMove, deltaTime);
        }

        if (_onCameraOrbitTrigger == true || _prevOnCameraOrbitTrigger == true)
        {
            OnCameraOrbit?.Invoke(_onCameraOrbitTrigger, _mousePos, deltaTime);
            _prevOnCameraOrbitTrigger = _onCameraOrbitTrigger;
        }
    }

    #region InputActions
    private void OnMousePos(InputValue inputValue)
    {
        _mousePos = inputValue.Get<Vector2>();
    }

    private void OnCameraMove(InputValue inputValue)
    {
        _cameraMove = inputValue.Get<Vector2>();
    }

    private void OnCameraOrbitTrigger(InputValue inputValue)
    {
        _onCameraOrbitTrigger = inputValue.Get<float>() == 1f;
    }

    private void OnSelect()
    {
        _actionTriggers[(int)ActionTriggerId.OnSelect].IsTriggered = true;
    }

    private void OnPlace()
    {
        _actionTriggers[(int)ActionTriggerId.OnPlace].IsTriggered = true;
    }

    private void OnCancel()
    {
        _actionTriggers[(int)ActionTriggerId.OnCancel].IsTriggered = true;
    }
    #endregion InputActions

    private void DoSelect()
    {
        Debug.Log("OnSelect");
    }

    private void DoPlace()
    {
        Debug.Log("OnPlace");
        // TODO: It should probably not be the InputController that actually spawns the unit...
        var go = Instantiate(UnitToPlace.UnitPrefab, MouseHitPosition, Quaternion.identity);
        SetState(InputState.Select);
    }

    private void DoCancel()
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
