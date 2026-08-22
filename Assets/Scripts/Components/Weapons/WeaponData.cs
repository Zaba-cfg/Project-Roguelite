using UnityEngine;

namespace Components.Weapons
{
    [CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private string _weaponName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private float _damage;
        [SerializeField, Min(0.01f)] private float _fireRate;
        [SerializeField, Min(1)] private int _magazineSize;
        [SerializeField] private WeaponFireStrategy _weaponFireStrategy;
        [SerializeField, Min(0f)] private float _reloadDuration;
        [SerializeField, Min(0f)] private int _maxReserveAmmo;
    
        public string WeaponName => _weaponName;
        public Sprite Icon => _icon;
        public float Damage => _damage;
        public float FireRate => _fireRate;
        public int MagazineSize => _magazineSize;
        public WeaponFireStrategy WeaponFireStrategy => _weaponFireStrategy;
        public float ReloadDuration => _reloadDuration;
        public int MaxReserveAmmo => _maxReserveAmmo;
    }
}
