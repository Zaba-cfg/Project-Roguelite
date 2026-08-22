using System.Collections.Generic;
using Components.Modifiers;

namespace Interfaces
{
    public interface IModifierProvider
    {
        IReadOnlyList<ModifierInstance> Modifiers { get; }
    }
}