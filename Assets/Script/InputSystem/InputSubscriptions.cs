using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class InputSubscriptions : MonoBehaviour
{

    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public Vector2 CursorInput { get; private set; } = Vector2.zero;
    public float cursorSpeed = 1000f;
    public Vector2 sensitivity = new Vector2(1500f, 1500f);
    public Vector2 bias = new Vector2(0f, -1f);
    Vector2 overflow;
    Vector2 warpPosition;

    public bool FireInput { get; private set; } = false;
    public bool SonarInput { get; private set; } = false;
    public bool DashInput { get; private set; } = false;
    public bool InteractInput { get; private set; } = false;

    private Mouse virtualMouse;
    private PlayerInput playerInput;

    IA_PlayerInputs _Input = null;

    WeaponBaseControl weaponBaseControlInstance;

    public static InputSubscriptions instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        if (instance == null)
            instance = this;
    }

    #region --- Virtual Mouse ---
    void VirtualMouseInit()
    {
        if(virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if(!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        InputState.Change(virtualMouse.position, Mouse.current.position.ReadValue());

        //InputSystem.onAfterUpdate += UpdateVirtualMouse;
    }

    private void UpdateVirtualMouse()
    {
        if (virtualMouse == null)
            return;

        Vector2 gameStickValue = Gamepad.current.rightStick.ReadValue();

        gameStickValue *= cursorSpeed * Time.unscaledDeltaTime;

        Vector2 virtualMouseCurrentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = virtualMouseCurrentPosition + gameStickValue;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, gameStickValue);
    }
    #endregion

    void GamepadToMouse()
    {
        Vector2 gameStickValue = CursorInput;
        if (gameStickValue.magnitude < 0.1f)
            return;

        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        warpPosition = mousePosition + bias + overflow + sensitivity * Time.unscaledDeltaTime * gameStickValue;
        warpPosition = new Vector2(Mathf.Clamp(warpPosition.x, 0, Screen.width), Mathf.Clamp(warpPosition.y, 0, Screen.height));
        overflow = new Vector2(warpPosition.x % 1, warpPosition.y % 1);

        Mouse.current.WarpCursorPosition(warpPosition);
    }
    private void OnEnable()
    {
        _Input = new IA_PlayerInputs();
        _Input.PlayerInputs.Enable();

        //VirtualMouseInit();

        //WASD & Controller Movement inputs ---
        _Input.PlayerInputs.Movements.performed += SetMovement;
        _Input.PlayerInputs.Movements.canceled += SetMovement;

        //WASD & Cursor inputs ---
        _Input.PlayerInputs.Cursor.performed += SetCursor;
        _Input.PlayerInputs.Cursor.canceled += SetCursor;

        //Fire weapon input ---
        _Input.PlayerInputs.Fire.started += SetFire;
        _Input.PlayerInputs.Fire.canceled += SetFire;

        //SonarScan input ---
        //_Input.PlayerInputs.SonarScan.started += SetSonar;
        //_Input.PlayerInputs.SonarScan.canceled += SetSonar;

        //Dash input ---
        _Input.PlayerInputs.Dash.started += SetDash;
        _Input.PlayerInputs.Dash.canceled += SetDash;

        //Interact input ---
        _Input.PlayerInputs.Interact.started += SetInteract;
        _Input.PlayerInputs.Interact.canceled += SetInteract;
    }
    private void OnDisable()
    {
        //InputSystem.RemoveDevice(virtualMouse);
        //InputSystem.onAfterUpdate -= UpdateVirtualMouse;

        _Input.PlayerInputs.Movements.performed -= SetMovement;
        _Input.PlayerInputs.Movements.canceled -= SetMovement;

        _Input.PlayerInputs.Cursor.performed -= SetCursor;
        _Input.PlayerInputs.Cursor.canceled -= SetCursor;

        _Input.PlayerInputs.Fire.started -= SetFire;
        _Input.PlayerInputs.Fire.canceled -= SetFire;

        //_Input.PlayerInputs.SonarScan.started -= SetSonar;
        //_Input.PlayerInputs.SonarScan.canceled -= SetSonar;

        _Input.PlayerInputs.Dash.started -= SetDash;
        _Input.PlayerInputs.Dash.canceled -= SetDash;

        _Input.PlayerInputs.Interact.started -= SetInteract;
        _Input.PlayerInputs.Interact.canceled -= SetInteract;

        _Input.PlayerInputs.Disable();
    }

    private void Start()
    {
        weaponBaseControlInstance = WeaponBaseControl.instance;
    }

    private void Update()
    {
        SonarInput = _Input.PlayerInputs.SonarScan.WasPressedThisFrame();
        DashInput = _Input.PlayerInputs.Dash.WasPressedThisFrame();
        InteractInput = _Input.PlayerInputs.Interact.WasPressedThisFrame();
        GamepadToMouse();
    }
    void SetMovement(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }
    void SetCursor(InputAction.CallbackContext ctx)
    {
        CursorInput = ctx.ReadValue<Vector2>();
    }

    void SetFire(InputAction.CallbackContext ctx)
    {
        FireInput = ctx.started;
        if (ctx.canceled)
            weaponBaseControlInstance.FireInputCanceled();
    }

    void SetSonar(InputAction.CallbackContext ctx)
    {
        SonarInput = ctx.started;
    }
    void SetDash(InputAction.CallbackContext ctx)
    {
        DashInput = ctx.started;
    }
    void SetInteract(InputAction.CallbackContext ctx)
    {
        InteractInput = ctx.started;
    }
}
