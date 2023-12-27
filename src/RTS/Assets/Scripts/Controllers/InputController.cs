using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class InputController : MonoBehaviour
{
    public static InputController Instance;

    public Camera CurrentCamera;

    #region events
    public event Action<Vector2, float> OnCameraPan;
    public event Action<bool, Vector2, float> OnCameraOrbit;
    public event Action<float> OnCameraDolly;

    public event Action<bool> OnSelectStart;
    public event Action<bool> OnSelectEnd;

    public event Action OnPlace;
    public event Action OnCancel;

    public event Action OnCommand;
    #endregion events

    /// <summary>
    /// True if the raycast into the scene hit something this frame.
    /// </summary>
    public bool HasMouseRayHit { get; protected set; }

    /// <summary>
    /// The world position where the raycast into the scene hit something. Only use if HasMouseHit is true.
    /// </summary>
    public Vector3 MouseRayHitPosition { get; protected set; }

    /// <summary>
    /// The GameObject the raycast into the scene hit. Only use if HasMouseHit is true.
    /// </summary>
    public GameObject MouseRayHitObject { get; protected set; }


    private PlayerInput _playerInput;
    public enum ActionMapId
    {
        Select,
        Place,
        Command
    }
    private string[] _actionMapNames =     // Must match InputState
    {
        "Select",
        "Place",
        "Command"
    };
    public ActionMapId CurrentActionMapId { get; protected set; }


    private Dictionary<string, Action<InputAction>> Actions;

    private Vector2 _mousePos;
    private Vector2 _mouseDelta;
    private Vector2 _cameraMove;
    private bool _onCameraOrbit;
    private bool _prevOnCameraOrbit;
    private float _cameraDolly;
    private bool _onSelect;
        
    public void SetActionMap(ActionMapId state)
    {
        CurrentActionMapId = state;
        _playerInput.currentActionMap = _playerInput.actions.FindActionMap(_actionMapNames[(int)CurrentActionMapId]);
    }

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("InputController.OnEnable: There is more than one instance");
            gameObject.SetActive(false);
            return;
        }
        Instance = this;

        if (Actions == null)
        {
            Actions = new Dictionary<string, Action<InputAction>>()
            {
                { "MousePosAction",   OnMousePosAction },
                { "MouseDeltaAction", OnMouseDeltaAction },

                { "CameraMoveAction",  OnCameraMoveAction  },
                { "CameraOrbitAction", OnCameraOrbitAction },
                { "CameraDollyAction", OnCameraDollyAction },

                { "SelectAction", OnSelectAction },
                { "AddSelectAction", OnAddSelectAction },
                { "PlaceAction",  OnPlaceAction  },
                { "CancelAction", OnCancelAction },

                { "CommandAction", OnCommandAction },
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

        SetActionMap(ActionMapId.Select);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime; // TODO: Add pause/speed control here

        Update_MouseRaycast();

        Update_RunActions();
        Update_Camera(deltaTime);

        #region Debug
        DebugInfoPanel.Log("ActionMap", CurrentActionMapId.ToString());
        if (HasMouseRayHit)
        {
            DebugInfoPanel.Log("MouseRayHit", MouseRayHitPosition.ToString());
            DebugInfoPanel.Log("MouseRayHitObj", MouseRayHitObject.name);
        }
        else
        {
            DebugInfoPanel.Remove("MouseRayHit");
            DebugInfoPanel.Remove("MouseRayHitObj");
        }
        #endregion Debug
    }

    private void Update_RunActions()
    {
        var map = _playerInput.currentActionMap;
        foreach (var action in map)
        {
            if ((action.IsPressed() || action.WasReleasedThisFrame()) && EventSystem.current.IsPointerOverGameObject() == false) // TODO: This is an ugly way to do it, what if the action is not triggered by a click or anything else that should be eaten by the UI? For example OnCancel whitch triggers on the escape key.
            {
                Actions[action.name](action);
            }
            if (action.WasReleasedThisFrame())
            {
                Actions[action.name](action);
            }
        }
    }

    private void Update_MouseRaycast()
    {
        bool hasMouseRayHit = false;

        if (Physics.Raycast(CurrentCamera.ScreenPointToRay(_mousePos), out RaycastHit hit)) // TODO: Why doesn't this hit UI elements?
        {
            MouseRayHitPosition = hit.point;
            MouseRayHitObject = hit.collider.gameObject;
            hasMouseRayHit = true;
        }

        HasMouseRayHit = hasMouseRayHit;
    }

    private void Update_Camera(float deltaTime)
    {
        if (_cameraMove.sqrMagnitude > 0f)
        {
            OnCameraPan?.Invoke(_cameraMove, deltaTime);
        }

        if (_onCameraOrbit == true || _prevOnCameraOrbit == true)
        {
            OnCameraOrbit?.Invoke(_onCameraOrbit, _mouseDelta, deltaTime);
            _prevOnCameraOrbit = _onCameraOrbit;
        }
    }

    #region InputActions
    private void OnMousePosAction(InputAction context)
    {
        _mousePos = context.ReadValue<Vector2>();
    }

    private void OnMouseDeltaAction(InputAction context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    private void OnCameraMoveAction(InputAction context)
    {
        _cameraMove = context.ReadValue<Vector2>();
    }
    private void OnCameraOrbitAction(InputAction context)
    {
        _onCameraOrbit = context.ReadValue<float>() == 1f;
    }

    private void OnCameraDollyAction(InputAction context)
    {
        _cameraDolly = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
        OnCameraDolly?.Invoke(_cameraDolly);
    }

    private void OnSelectAction(InputAction context)
    {
        var isSelecting = context.IsPressed();

        switch (_onSelect)
        {
            case false when isSelecting == true:
                OnSelectStart?.Invoke(false);
                break;
            case true when isSelecting == false:
                OnSelectEnd?.Invoke(false);
                break;
        }

        _onSelect = isSelecting;
    }
    private void OnAddSelectAction(InputAction context)
    {
        var isSelecting = context.IsPressed();

        switch (_onSelect)
        {
            case false when isSelecting == true:
                OnSelectStart?.Invoke(true);
                break;
            case true when isSelecting == false:
                OnSelectEnd?.Invoke(true);
                break;
        }

        _onSelect = isSelecting;
    }

    private void OnPlaceAction(InputAction context)
    {
        OnPlace?.Invoke();
    }

    private void OnCancelAction(InputAction context)
    {
        OnCancel?.Invoke();
    }

    private void OnCommandAction(InputAction context)
    {
        OnCommand?.Invoke();
    }
    #endregion InputActions
}
