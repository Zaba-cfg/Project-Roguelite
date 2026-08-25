using UnityEngine;

namespace Components.Weapons
{
    public class WeaponFireContext
    {
        public Weapon Weapon { get; }
        public Vector2 Direction { get; set;  }
        public int AttackCount { get; set; } = 1;
        public float SpreadAngle { get; set; } = 0f;

        public WeaponFireContext(Weapon weapon, Vector2 direction)
        {
            Weapon = weapon;
            Direction = direction;
        }
    }
}