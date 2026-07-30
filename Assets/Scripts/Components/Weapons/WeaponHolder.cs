using System;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Transform _weaponSocket;
    public Weapon CurrentWeapon { get; private set; }

    private void Awake()
    {
        if (_weaponSocket == null) throw new MissingReferenceException($"{name} is missing a socket.");
    }

    public void Equip(Weapon weapon)
    {
        if (CurrentWeapon == weapon) return;
        
        if (weapon == null) throw new ArgumentNullException(nameof(weapon));
        
        if (weapon.IsEquipped) weapon.CurrentHolder.DropCurrentWeapon();

        CurrentWeapon = weapon;
        
        weapon.OnEquipped(this);
        
        Transform weaponTransform = weapon.transform;
        
        weaponTransform.SetParent(_weaponSocket);
        
        weaponTransform.localPosition = Vector3.zero;
        
        weaponTransform.localRotation = Quaternion.identity;
    }

    public void DropCurrentWeapon()
    {
        if (CurrentWeapon == null)
        {
            throw new InvalidOperationException($"{name} does not have a weapon to drop.");
        }

        Weapon weapon = CurrentWeapon;
        
        weapon = null;
        
        weapon.OnDropped();
        
        weapon.transform.SetParent(null);
        
    }
}
