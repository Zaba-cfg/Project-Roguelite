using Components.Base;
using Components.Base.HealthRelated;
using Components.Base.Weapon;
using Components.Enemy;
using Components.Enemy.EnemyBehavior;
using Components.Weapons;
using UnityEngine;

namespace Entities.Enemy
{
    [RequireComponent(typeof(EnemyAIInput))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(LookDirection))]
    [RequireComponent(typeof(WeaponHolder))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(EnemyCombat))]
    [RequireComponent(typeof(WeaponDetection))]
    [RequireComponent(typeof(EnemyWeaponDecision))]
    [RequireComponent(typeof(EnemyBehavior))]

    public class Enemy : MonoBehaviour
    {
    }
}
