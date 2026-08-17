using System.Collections.Generic;

public static class ModifierCalculator
{
    public static float Calculate(float baseValue, ModifierStat stat, params IModifierProvider[] providers)
    {
        float result = baseValue;

        foreach (IModifierProvider provider in providers)
        {
            if (provider == null)
                continue;

            foreach (Modifier modifier in provider.Modifiers)
            {
                if (modifier is not StatModifier statModifier)
                    continue;

                if (statModifier.Stat != stat)
                    continue;

                if (statModifier.Operation != ModifierOperation.Add)
                    continue;

                result += statModifier.Value;
            }
        }

        foreach (IModifierProvider provider in providers)
        {
            if (provider == null)
                continue;

            foreach (Modifier modifier in provider.Modifiers)
            {
                if (modifier is not StatModifier statModifier)
                    continue;

                if (statModifier.Stat != stat)
                    continue;

                if (statModifier.Operation != ModifierOperation.Multiply)
                    continue;

                result *= statModifier.Value;
            }
        }

        return result;
    }
}