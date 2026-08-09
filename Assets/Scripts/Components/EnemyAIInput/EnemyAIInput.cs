using UnityEngine;

public class EnemyAIInput : MonoBehaviour, IMoveInput
{
    public Vector2 MoveInput { get; private set; }

    [SerializeField] private Transform _target;

    private void Awake()
    {
        if (_target == null) throw new MissingReferenceException($"{name} is missing a target.");
    }

    private void Update()
    {
        MoveInput = _target.position - transform.position;
    }
}