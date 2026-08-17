using System.Collections.Generic;

public interface IModifierProvider
{
    IReadOnlyList<ModifierInstance> Modifiers { get; }
}