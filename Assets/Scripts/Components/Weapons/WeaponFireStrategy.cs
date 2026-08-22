using UnityEngine;

namespace Components.Weapons
{
    public abstract class WeaponFireStrategy : ScriptableObject
    {
        public abstract void Execute(Weapon weapon, Vector2 direction);
    }
}
