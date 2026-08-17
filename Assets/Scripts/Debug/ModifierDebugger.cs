using UnityEngine;

[RequireComponent(typeof(ModifierInventory))]
public class ModifierDebugger : MonoBehaviour
{
    private ModifierInventory _modifierInventory;

    private void Awake()
    {
        _modifierInventory = GetComponent<ModifierInventory>();
    }

    private void OnEnable()
    {
        _modifierInventory.ModifierAdded += OnModifierAdded;
        _modifierInventory.ModifierRemoved += OnModifierRemoved;
    }

    private void OnDisable()
    {
        _modifierInventory.ModifierAdded -= OnModifierAdded;
        _modifierInventory.ModifierRemoved -= OnModifierRemoved;
    }

    private void OnModifierAdded(ModifierInstance instance)
    {
        Debug.Log($"Modifier Added: {GetModifierDescription(instance)}");
    }

    private void OnModifierRemoved(ModifierInstance instance)
    {
        Debug.Log($"Modifier Removed: {GetModifierDescription(instance)}");
    }

    private string GetModifierDescription(ModifierInstance instance)
    {
        if (instance == null)
            return "Null";

        if (instance.Definition is StatModifierDefinition modifier)
        {
            string duration = instance.IsTemporary
                ? $"Temporary ({instance.ExpirationTime - Time.time:F1}s remaining)"
                : "Permanent";

            return $"{modifier.Stat} / {modifier.Operation} / {modifier.Value} / {duration}";
        }

        return instance.Definition.name;
    }
}