using UnityEngine;

[RequireComponent(typeof(WeaponDetection))]
[RequireComponent(typeof(WeaponHolder))]

public class EnemyWeaponDecision : MonoBehaviour
{
    private WeaponDetection _weaponDetection;
    private WeaponHolder _weaponHolder;

    private void Awake()
    {
        _weaponDetection = GetComponent<WeaponDetection>();
        _weaponHolder = GetComponent<WeaponHolder>();
    }

    private void Update()
    {
        if (_weaponHolder.HasWeapon)
            return;
        
        Weapon closestWeapon = null;
        float closestDistance = float.MaxValue;
        
        foreach (Weapon weapon in _weaponDetection.Weapons)
        {
            if (weapon.IsEquipped)
                continue;

            float distance = (weapon.transform.position - transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWeapon = weapon;
            }
        }
        if (closestWeapon == null)
            return;
        
        _weaponHolder.Equip(closestWeapon);
    }
}
