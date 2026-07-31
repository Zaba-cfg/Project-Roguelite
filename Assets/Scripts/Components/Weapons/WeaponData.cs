using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private string _weaponName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _damage;
    [SerializeField] private float _fireRate;
    [SerializeField] private int _magazineSize;
    [SerializeField] private WeaponFireStrategy _weaponFireStrategy;
    
    public string WeaponName => _weaponName;
    public Sprite Icon => _icon;
    public float Damage => _damage;
    public float FireRate => _fireRate;
    public int MagazineSize => _magazineSize;
    public WeaponFireStrategy WeaponFireStrategy => _weaponFireStrategy;
}
