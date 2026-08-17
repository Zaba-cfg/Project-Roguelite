using System;
using System.Collections.Generic;
using UnityEngine;

public class ModifierController : MonoBehaviour, IModifierProvider
{
    private readonly List<Modifier> _modifiers = new();

    public IReadOnlyList<Modifier> Modifiers => _modifiers;
    
    public void AddModifier(Modifier modifier)
    {
        if (modifier == null)
            throw new ArgumentNullException(nameof(modifier));

        if (_modifiers.Contains(modifier))
            return;

        _modifiers.Add(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        if (modifier == null)
            return;

        _modifiers.Remove(modifier);
    }
    
    public float CalculateValue(float baseValue, ModifierStat stat)
    {
        return ModifierCalculator.Calculate(baseValue, stat, this);
    }
}