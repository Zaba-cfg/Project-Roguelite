using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(LookDirection))]
[RequireComponent(typeof(WeaponHolder))]

public class Player : MonoBehaviour
{
    private PlayerInput _playerInput;
    private LookDirection _lookDirection;
    private WeaponHolder _weaponHolder;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();
        _lookDirection = GetComponent<LookDirection>();
        _weaponHolder = GetComponent<WeaponHolder>();
    }
    
    private void Update()
    {
        UpdateLookDirection();
    }

    private void UpdateLookDirection()
    {
        if (_playerInput.CurrentAimDevice == AimDevice.Gamepad)
        {
            if (_playerInput.LookStick != Vector2.zero)
            {
                _lookDirection.SetDirection(_playerInput.LookStick);
            }

            return;
        }

        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(_playerInput.LookPointer);

        Vector2 direction = (Vector2)mouseWorldPosition - (Vector2)transform.position;

        _lookDirection.SetDirection(direction);
    }
    
    private void HandleFire()
    {
        _weaponHolder.TryFire(_lookDirection.Forward);
    }
    
    private void OnEnable()
    {
        _playerInput.FirePressed += HandleFire;
    }

    private void OnDisable()
    {
        _playerInput.FirePressed -= HandleFire;
    }
}
