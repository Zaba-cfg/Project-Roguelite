using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, IMoveInput
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookPointer { get; private set; }
    public Vector2 LookStick { get; private set; }
    public AimDevice CurrentAimDevice { get; private set; } = AimDevice.Mouse;
    
    public event Action FirePressed;
    
    private PlayerInputActions _playerInputActions;
    private Vector2 _lastPointerPosition;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }
    
    private void OnFirePerformed(InputAction.CallbackContext obj)
    {
        FirePressed?.Invoke();
    }
    
    private void OnMovePerformed(InputAction.CallbackContext obj)
    {
        MoveInput = obj.ReadValue<Vector2>();
    }
    
    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        MoveInput = Vector2.zero;
    }

    private void OnLookPointerPerformed(InputAction.CallbackContext obj)
    {
        Vector2 newPosition = obj.ReadValue<Vector2>();

        if (newPosition != _lastPointerPosition)
        {
            CurrentAimDevice = AimDevice.Mouse;
            _lastPointerPosition = newPosition;
        }

        LookPointer = newPosition;
    }
    
    private void OnLookStickPerformed(InputAction.CallbackContext obj)
    {
        LookStick = obj.ReadValue<Vector2>();

        if (LookStick != Vector2.zero)
        {
            CurrentAimDevice = AimDevice.Gamepad;
        }
    }
    
    private void OnLookStickCanceled(InputAction.CallbackContext obj)
    {
        LookStick = Vector2.zero;
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Fire.performed += OnFirePerformed;
        _playerInputActions.Player.Move.performed += OnMovePerformed;
        _playerInputActions.Player.Move.canceled += OnMoveCanceled;
        _playerInputActions.Player.LookPointer.performed += OnLookPointerPerformed;
        _playerInputActions.Player.LookStick.performed += OnLookStickPerformed;
        _playerInputActions.Player.LookStick.canceled += OnLookStickCanceled;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Fire.performed -= OnFirePerformed;
        _playerInputActions.Player.Move.performed -= OnMovePerformed;
        _playerInputActions.Player.Move.canceled -= OnMoveCanceled;
        _playerInputActions.Player.LookPointer.performed -= OnLookPointerPerformed;
        _playerInputActions.Player.LookStick.performed -= OnLookStickPerformed;
        _playerInputActions.Player.LookStick.canceled -= OnLookStickCanceled;
        _playerInputActions.Disable();
    }
    
    private void OnDestroy()
    {
        _playerInputActions.Dispose();
    }
}
