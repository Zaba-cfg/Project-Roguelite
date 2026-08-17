using System;
using UnityEngine;

public class ModifierInstance
{
    public ModifierDefinition Definition { get; }
    public bool IsTemporary { get; }
    public float ExpirationTime { get; private set; }

    public ModifierInstance(ModifierDefinition definition)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        Definition = definition;
        IsTemporary = false;
    }

    public ModifierInstance(ModifierDefinition definition, float duration)
    {
        if (definition == null)
            throw new ArgumentNullException(nameof(definition));

        if (duration <= 0f)
            throw new ArgumentOutOfRangeException(nameof(duration));

        Definition = definition;
        IsTemporary = true;
        ExpirationTime = Time.time + duration;
    }

    public void Refresh(float duration)
    {
        if (!IsTemporary)
            throw new InvalidOperationException("Permanent modifiers cannot be refreshed.");

        if (duration <= 0f)
            throw new ArgumentOutOfRangeException(nameof(duration));

        ExpirationTime = Time.time + duration;
    }
}