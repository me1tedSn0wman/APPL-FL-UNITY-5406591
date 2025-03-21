using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public enum ControlType { 
    General,
}

public class PlayerControlManager : Soliton<PlayerControlManager>
{
    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }
    public bool sprint { get; private set; }
    public bool jump { get; private set; }
    public bool interact { get; private set; }
    public bool crouch { get; private set; }
    public bool menu { get; private set; }

    public event Action OnMenuButtonPressed;
    public event Action OnInteractionPressed;
    public event Action OnCrouchPressed;

    [SerializeField] private float buttonDeadZone = 0.1f;
    [SerializeField] private float normalLookMouseMult = 0.02f;
    [SerializeField] private float normalLookGamepadMult = 0.02f;

    private PlayerInput playerInput;

    [SerializeField] private ControlType _controlType;
    public ControlType controlType {
        set {
            switch (value) {
                case ControlType.General:
                    playerInput.SwitchCurrentActionMap("Gameplay");
                    break;
                default:
                    playerInput.SwitchCurrentActionMap("Gameplay");
                    break;
            }
            _controlType = value;
        }
        get { return _controlType; }
    }

    public override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
    }

    public void Start()
    {
        controlType = ControlType.General;
        SetCursorActive(false);
    }

    public void OnMove(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>();
    }

    public void OnLookMouse(InputAction.CallbackContext context) { 
        look = normalLookMouseMult * context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) { 
        look = normalLookGamepadMult * context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context) {
        sprint = context.ReadValue<float>() > buttonDeadZone;
    }

    public void OnJump(InputAction.CallbackContext context) {
        jump = context.ReadValue<float>() > buttonDeadZone;
    }

    public void OnMenu(InputAction.CallbackContext context) {
        bool val = context.ReadValue<float>() > buttonDeadZone;
        if (menu == false && val == true)
        {
            menu = true;
            OnMenuButtonPressed?.Invoke();
        }
        else if (val == false) {
            menu = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context) {
        bool val = context.ReadValue<float>() > buttonDeadZone;
        if (interact == false && val == true)
        {
            interact = true;
            OnInteractionPressed?.Invoke();
        }
        else if (val == false) {
            interact = false;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        bool val = context.ReadValue<float>() > buttonDeadZone;
        if (crouch == false && val == true)
        {
            crouch = true;
            OnInteractionPressed?.Invoke();
        }
        else if (val == false)
        {
            crouch = false;
        }
    }

    public static void SetCursorActive(bool value)
    {
        if (value)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
