using System.Collections.Generic;

public interface IModifierProvider
{
    IReadOnlyList<Modifier> Modifiers { get; }
}