using Interfaces;
using UnityEngine;

namespace Components.Modifiers
{
    [RequireComponent(typeof(Collider2D))]
    public class ModifierPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private ModifierDefinition _modifier;

        public void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out ModifierInventory modifierInventory))
                return;

            if (!modifierInventory.AddModifier(_modifier))
                return;

            Destroy(gameObject);
        }
    }
}