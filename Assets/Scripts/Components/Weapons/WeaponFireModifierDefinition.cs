using Components.Modifiers;

namespace Components.Weapons
{
    public abstract class WeaponFireModifierDefinition : ModifierDefinition
    {
        public abstract void Modify(WeaponFireContext context);
    }
}