using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Components.Modifiers
{
    public class ModifierInventory : MonoBehaviour, IModifierProvider
    {
        private readonly List<ModifierInstance> _modifiers = new();

        public IReadOnlyList<ModifierInstance> Modifiers => _modifiers;

        public event Action<ModifierInstance> ModifierAdded;
        public event Action<ModifierInstance> ModifierRemoved;

        private void Update()
        {
            List<ModifierInstance> expiredModifiers = new();

            foreach (ModifierInstance instance in _modifiers)
            {
                if (!instance.IsTemporary)
                    continue;

                if (Time.time >= instance.ExpirationTime)
                {
                    expiredModifiers.Add(instance);
                }
            }

            foreach (ModifierInstance instance in expiredModifiers)
            {
                RemoveModifier(instance);
            }
        }

        public bool AddModifier(ModifierDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            ModifierInstance instance = new ModifierInstance(definition);

            _modifiers.Add(instance);

            ModifierAdded?.Invoke(instance);

            return true;
        }

        public bool AddTemporaryModifier(ModifierDefinition definition, float duration)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (duration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(duration));

            ModifierInstance instance =
                new ModifierInstance(definition, duration);

            _modifiers.Add(instance);

            ModifierAdded?.Invoke(instance);

            return true;
        }

        public bool RemoveModifier(ModifierInstance instance)
        {
            if (instance == null)
                return false;

            if (!_modifiers.Remove(instance))
                return false;

            ModifierRemoved?.Invoke(instance);

            return true;
        }

        public float CalculateValue(float baseValue, ModifierStat stat)
        {
            return ModifierCalculator.Calculate(baseValue, stat, this);
        }
    }
}