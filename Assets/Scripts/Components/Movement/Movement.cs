using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private IMoveInput _moveInput;
    
    [SerializeField] private float _speed = 5f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _moveInput = GetComponent<IMoveInput>();
        
        if (_moveInput == null)
            throw new MissingComponentException($"{name} requires a component implementing IMoveInput.");
    }

    private void FixedUpdate()
    {
        var inputValue = Vector2.ClampMagnitude(_moveInput.MoveInput,1f);
        _rigidbody.linearVelocity = inputValue * _speed;
    }
}
