using UnityEngine;

[RequireComponent(typeof(EnemyAIInput))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(LookDirection))]
[RequireComponent(typeof(WeaponHolder))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(EnemyCombat))]

public class Enemy : MonoBehaviour
{
    private LookDirection _lookDirection;
    private EnemyAIInput _enemyAIInput;

    private void Awake()
    {
        _lookDirection = GetComponent<LookDirection>();
        _enemyAIInput = GetComponent<EnemyAIInput>();
    }

    private void Update()
    {
        UpdateLookDirection();
    }

    private void UpdateLookDirection()
    {
        _lookDirection.SetDirection(_enemyAIInput.MoveInput);
    }
}
