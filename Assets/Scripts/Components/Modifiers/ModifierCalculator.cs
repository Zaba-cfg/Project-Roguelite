using System.Collections.Generic;

public static class ModifierCalculator
{
    public static float Calculate(float baseValue, ModifierStat stat, IReadOnlyList<Modifier> modifiers)
    {
        float result = baseValue;

        foreach (Modifier modifier in modifiers)
        {
            if (modifier is not StatModifier statModifier)
                continue;

            if (statModifier.Stat != stat)
                continue;

            if (statModifier.Operation != ModifierOperation.Add)
                continue;

            result += statModifier.Value;
        }

        foreach (Modifier modifier in modifiers)
        {
            if (modifier is not StatModifier statModifier)
                continue;

            if (statModifier.Stat != stat)
                continue;

            if (statModifier.Operation != ModifierOperation.Multiply)
                continue;

            result *= statModifier.Value;
        }

        return result;
    }
}