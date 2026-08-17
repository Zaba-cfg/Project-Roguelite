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

            foreach (ModifierInstance instance in provider.Modifiers)
            {
                if (instance?.Definition is not StatModifierDefinition modifier)
                    continue;

                if (modifier.Stat != stat)
                    continue;

                if (modifier.Operation != ModifierOperation.Add)
                    continue;

                result += modifier.Value;
            }
        }

        foreach (IModifierProvider provider in providers)
        {
            if (provider == null)
                continue;

            foreach (ModifierInstance instance in provider.Modifiers)
            {
                if (instance?.Definition is not StatModifierDefinition modifier)
                    continue;

                if (modifier.Stat != stat)
                    continue;

                if (modifier.Operation != ModifierOperation.Multiply)
                    continue;

                result *= modifier.Value;
            }
        }

        return result;
    }
}