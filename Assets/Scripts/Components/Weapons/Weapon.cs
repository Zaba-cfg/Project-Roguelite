using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private Transform _muzzle;
    
    private WeaponHolder _currentHolder;
    private float _nextFireTime;
    
    public WeaponHolder CurrentHolder => _currentHolder;
    public bool IsEquipped => _currentHolder != null;
    public WeaponData WeaponData => _weaponData;
    public float Damage => WeaponData.Damage;
    public Transform Muzzle => _muzzle;
    public GameObject Owner => CurrentHolder.gameObject;
    
    public int CurrentAmmo { get; private set; }

    private void Awake()
    {
        if (_weaponData == null)
            throw new MissingReferenceException($"{name} is missing WeaponData");

        if (_muzzle == null)
            throw new MissingReferenceException($"{name} is missing Muzzle");
        
        CurrentAmmo = _weaponData.MagazineSize;
    }

    public WeaponFireResult TryFire(Vector2 direction)
    {
        if (!IsEquipped) return WeaponFireResult.NotEquipped;

        if (Time.time < _nextFireTime) return WeaponFireResult.Cooldown;

        if (CurrentAmmo <= 0) return WeaponFireResult.NoAmmo;
        
        CurrentAmmo--;

        _nextFireTime = Time.time + (1f / _weaponData.FireRate);
        
        _weaponData.WeaponFireStrategy.Execute(this, direction);

        return WeaponFireResult.Success;
    }

    public void OnEquipped(WeaponHolder holder)
    {
        _currentHolder = holder;
    }

    public void OnDropped()
    {
        _currentHolder = null;
    }
}
