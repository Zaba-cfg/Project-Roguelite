using Components.Weapons;
using UnityEngine;

namespace Components.Modifiers
{
    [CreateAssetMenu(fileName = "Double Shot Modifier", menuName = "Modifiers/Weapon Fire/Double Shot" )]
    
    public class DoubleShotModifierDefinition : WeaponFireModifierDefinition
    {
        public override void Modify(WeaponFireContext context)
        {
            context.ProjectileCount++;
            context.SpreadAngle = 10f;
        }
    }
}