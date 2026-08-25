using Components.Weapons;
using Interfaces;
using UnityEngine;

namespace Components.Base.Weapon
{
    [RequireComponent(typeof(Weapons.Weapon))]
    [RequireComponent(typeof(Interaction))]

    public class WeaponPickup : MonoBehaviour, IInteractable
    {
        private Weapons.Weapon _weapon;

        private void Awake()
        {
            _weapon = GetComponent<Weapons.Weapon>();
        }

        public void Interact(GameObject interactor)
        {
            if (!interactor.TryGetComponent(out WeaponHolder weaponHolder))
                return;

            weaponHolder.Equip(_weapon);
        }
    }
}
