using Components.Weapons;
using Interfaces;
using UnityEngine;

namespace Components.WeaponPickup
{
    [RequireComponent(typeof(Weapon))]
    [RequireComponent(typeof(Interaction.Interaction))]

    public class WeaponPickup : MonoBehaviour, IInteractable
    {
        private Weapon _weapon;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
        }

        public void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out WeaponHolder weaponHolder))
                return;

            weaponHolder.Equip(_weapon);
        }
    }
}
