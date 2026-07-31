using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Fire Strategy", menuName = "Projectiles/Projectile Fire Strategy")]

public class ProjectileFireStrategy : WeaponFireStrategy
{
    [SerializeField] private Projectile _projectile;

    public override void Execute(Weapon weapon, Vector2 direction)
    {
        if (_projectile == null) throw new InvalidOperationException($"{name} is missing a projectile");
        
        Projectile newProjectile = Instantiate(_projectile, weapon.Muzzle.position, Quaternion.identity);
        
        newProjectile.Initialize(direction, weapon.Damage, weapon.Owner);
    }
}
