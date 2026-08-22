using Components.Weapons;
using UnityEngine;

namespace Components.EnemyBehavior
{
    [RequireComponent(typeof(EnemyAIInput.EnemyAIInput))]
    [RequireComponent(typeof(EnemyCombat.EnemyCombat))]
    [RequireComponent(typeof(EnemyWeaponDecision.EnemyWeaponDecision))]
    [RequireComponent(typeof(LookDirection.LookDirection))]
    [RequireComponent(typeof(WeaponHolder))]

    public class EnemyBehavior : MonoBehaviour
    {
        public EnemyState CurrentState { get; private set; }
    
        private EnemyAIInput.EnemyAIInput _aiInput;
        private EnemyCombat.EnemyCombat _combat;
        private EnemyWeaponDecision.EnemyWeaponDecision _weaponDecision;
        private LookDirection.LookDirection _lookDirection;
        private WeaponHolder _weaponHolder;
    
        [SerializeField] private Transform _target;

        private void Awake()
        {
            if (_target == null)
                throw new MissingReferenceException($"{name} is missing a target.");
        
            _aiInput = GetComponent<EnemyAIInput.EnemyAIInput>();
            _combat = GetComponent<EnemyCombat.EnemyCombat>();
            _weaponDecision = GetComponent<EnemyWeaponDecision.EnemyWeaponDecision>();
            _lookDirection = GetComponent<LookDirection.LookDirection>();
            _weaponHolder = GetComponent<WeaponHolder>();

            CurrentState = EnemyState.Chasing;
        }
    
        private void Update()
        {
            switch (CurrentState)
            {
                case EnemyState.Chasing:
                    HandleChasing();
                    break;

                case EnemyState.SeekingWeapon:
                    HandleSeekingWeapon();
                    break;

                case EnemyState.Attacking:
                    HandleAttacking();
                    break;
            }
        }

        private void HandleChasing()
        {
            _aiInput.SetTarget(_target);
            _lookDirection.SetDirection(_target.position - transform.position);
        
            if (!_weaponHolder.HasWeapon)
            {
                CurrentState = EnemyState.SeekingWeapon;
                return;
            }

            if (_weaponHolder.CurrentWeapon.IsEmpty)
            {
                _weaponHolder.DropCurrentWeapon();
                CurrentState = EnemyState.SeekingWeapon;
                return;
            }
        
            if (_combat.CanAttack())
            {
                CurrentState = EnemyState.Attacking;
                return;
            }
        }
    
        private void HandleSeekingWeapon()
        {
            Weapon weapon = _weaponDecision.GetClosestAvailableWeapon();

            if (weapon == null)
            {
                CurrentState = EnemyState.Chasing;
                return;
            }

            _aiInput.SetTarget(weapon.transform);
            _lookDirection.SetDirection(weapon.transform.position - transform.position);

            if (!_weaponDecision.IsWeaponInPickupRange(weapon))
                return;

            if (_weaponDecision.TryEquipNearbyWeapon())
            {
                CurrentState = EnemyState.Chasing;
            }
        }
    
        private void HandleAttacking()
        {
            if (!_weaponHolder.HasWeapon)
            {
                CurrentState = EnemyState.SeekingWeapon;
                return;
            }

            Weapon weapon = _weaponHolder.CurrentWeapon;

            if (weapon.IsEmpty)
            {
                _weaponHolder.DropCurrentWeapon();
                CurrentState = EnemyState.SeekingWeapon;
                return;
            }

            _lookDirection.SetDirection(_target.position - transform.position);

            if (!_combat.CanAttack())
            {
                CurrentState = EnemyState.Chasing;
                return;
            }
        
            if (weapon.CurrentAmmo <= 0)
            {
                weapon.Reload();
                return;
            }

            _combat.Attack(_lookDirection.Forward);
        }
    }
}
