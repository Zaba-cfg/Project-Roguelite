using System;
using Components.Modifiers;
using Interfaces;
using UnityEngine;

namespace Components.Weapons
{
    [RequireComponent(typeof(ModifierInventory))]

    public class WeaponHolder : MonoBehaviour
    {
        private ModifierInventory _modifierInventory;
        public IModifierProvider ModifierProvider => _modifierInventory;
    
        public event Action<Weapon> WeaponChanged;
    
        [SerializeField] private Transform _weaponSocket;
        public Weapon CurrentWeapon { get; private set; }
    
        public bool HasWeapon => CurrentWeapon != null;

        private void Awake()
        {
            if (_weaponSocket == null) throw new MissingReferenceException($"{name} is missing a socket.");
        
            _modifierInventory = GetComponent<ModifierInventory>();
        }
    
        public void Reload()
        {
            if (CurrentWeapon == null)
                return;

            CurrentWeapon.Reload();
        }

        public void Equip(Weapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));

            if (CurrentWeapon == weapon)
                return;

            if (CurrentWeapon != null)
                DropCurrentWeapon();

            if (weapon.IsEquipped)
                weapon.CurrentHolder.DropCurrentWeapon();

            CurrentWeapon = weapon;

            weapon.OnEquipped(this);

            Transform weaponTransform = weapon.transform;

            weaponTransform.SetParent(_weaponSocket);

            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
        
            WeaponChanged?.Invoke(weapon);
        }

        public void DropCurrentWeapon()
        {
            if (CurrentWeapon == null)
                throw new InvalidOperationException($"{name} does not have a weapon to drop.");

            Weapon weapon = CurrentWeapon;

            CurrentWeapon = null;

            weapon.OnDropped();

            weapon.transform.SetParent(null);
        
            WeaponChanged?.Invoke(null);
        }

        public void TryFire(Vector2 direction)
        {
            if (CurrentWeapon == null) return;
        
            CurrentWeapon.TryFire(direction);
        }
    }
}
