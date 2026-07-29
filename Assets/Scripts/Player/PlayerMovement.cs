using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    
    [SerializeField] private float _speed = 5f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        var inputValue = Vector2.ClampMagnitude(_playerInput.MoveInput,1);
        _rigidbody.linearVelocity = inputValue * _speed;
    }
}
