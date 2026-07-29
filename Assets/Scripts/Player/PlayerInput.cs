using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }
    
    private void OnMovePerformed(InputAction.CallbackContext obj)
    {
        MoveInput = obj.ReadValue<Vector2>();
        Debug.Log(MoveInput);
    }
    
    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        MoveInput = obj.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.Move.performed += OnMovePerformed;
        _playerInputActions.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Move.performed -= OnMovePerformed;
        _playerInputActions.Player.Move.canceled -= OnMoveCanceled;
        _playerInputActions.Disable();
    }
    
    private void OnDestroy()
    {
        _playerInputActions.Dispose();
    }
}
