using Interfaces;
using UnityEngine;

namespace Components.Modifiers
{
    [RequireComponent(typeof(Collider2D))]
    public class TemporaryModifierPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private ModifierDefinition _modifier;
        [SerializeField] private float _duration = 10f;

        public void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out ModifierInventory modifierInventory))
                return;

            if (!modifierInventory.AddTemporaryModifier(_modifier, _duration))
                return;

            Destroy(gameObject);
        }
    }
}