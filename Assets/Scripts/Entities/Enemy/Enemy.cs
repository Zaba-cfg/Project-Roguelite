using Components.EnemyAIInput;
using Components.EnemyBehavior;
using Components.EnemyCombat;
using Components.EnemyWeaponDecision;
using Components.Health;
using Components.LookDirection;
using Components.Movement;
using Components.WeaponDetection;
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
