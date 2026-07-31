using UnityEngine;

public class EnemyAIInput : MonoBehaviour, IMoveInput
{
    public Vector2 MoveInput { get; private set; }
    private Player _player;
    private Transform _playerTransform;

    private void Awake()
    {
        _player = FindFirstObjectByType<Player>();
        if (_player == null)
        {
            throw new MissingComponentException($"{name} requires a component implementing Player.");
        }
        _playerTransform = _player.transform;
    }

    private void Update()
    {
        MoveInput = _playerTransform.position - transform.position;
    }
}
