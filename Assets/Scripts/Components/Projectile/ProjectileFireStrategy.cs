using System;
using Components.Weapons;
using UnityEngine;

namespace Components.Projectile
{
    [CreateAssetMenu(fileName = "Projectile Fire Strategy", menuName = "Projectiles/Projectile Fire Strategy")]

    public class ProjectileFireStrategy : WeaponFireStrategy
    {
        [SerializeField] private Projectile _projectile;

        public override void Execute(WeaponFireContext context)
        {
            if (!_projectile)
                throw new InvalidOperationException($"{name} is missing a projectile.");
            
            float angleStep = context.ProjectileCount > 1 ? context.SpreadAngle / (context.ProjectileCount - 1) : 0f;
            
            float startAngle = -context.SpreadAngle / 2f;

            for (int i = 0; i < context.ProjectileCount; i++)
            {
                float angle = startAngle + angleStep * i;
                
                Vector2 projectileDirection = Quaternion.Euler(0f, 0f, angle) * context.Direction;
                
                Projectile newProjectile = Instantiate(_projectile, context.Weapon.Muzzle.position, Quaternion.identity);

                newProjectile.Initialize(projectileDirection, context.Weapon.Damage, context.Weapon.Owner);
            }
        }
    }
}
