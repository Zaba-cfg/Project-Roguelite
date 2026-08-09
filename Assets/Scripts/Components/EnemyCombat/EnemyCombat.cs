using UnityEngine;

[RequireComponent(typeof(WeaponHolder))]
[RequireComponent(typeof(LookDirection))]
public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _attackRange = 5f;
    private WeaponHolder _weaponHolder;
    private LookDirection _lookDirection;

    private void Awake()
    {
        if (_target == null) 
            throw new MissingReferenceException($"{name} is missing a target.");
        
        _weaponHolder = GetComponent<WeaponHolder>();
        _lookDirection = GetComponent<LookDirection>();
    }

    private void Update()
    {
        float distanceSquared = (_target.position - transform.position).sqrMagnitude;
        
        if (distanceSquared > _attackRange * _attackRange) 
            return;
        
        _weaponHolder.TryFire(_lookDirection.Forward);
    }
}
