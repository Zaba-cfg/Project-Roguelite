using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public event Action WeaponFired;
    public event Action<int, int> AmmoChanged;
    public event Action ReloadStarted;
    public event Action ReloadCompleted;
    
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private Transform _muzzle;
    
    private WeaponHolder _currentHolder;
    private float _nextFireTime;
    private float _reloadEndTime;
    
    public bool IsEmpty => CurrentAmmo <= 0 && ReserveAmmo <= 0;
    public WeaponHolder CurrentHolder => _currentHolder;
    public bool IsEquipped => _currentHolder != null;
    public WeaponData WeaponData => _weaponData;
    public float Damage => WeaponData.Damage;
    public Transform Muzzle => _muzzle;
    public GameObject Owner => CurrentHolder.gameObject;
    
    public int CurrentAmmo { get; private set; }
    public bool IsReloading { get; private set; }
    public int ReserveAmmo { get; private set; }

    private void Awake()
    {
        if (_weaponData == null)
            throw new MissingReferenceException($"{name} is missing WeaponData");

        if (_muzzle == null)
            throw new MissingReferenceException($"{name} is missing Muzzle");
        
        CurrentAmmo = _weaponData.MagazineSize;
        ReserveAmmo = _weaponData.MaxReserveAmmo;
    }

    private void Update()
    {
        if (!IsReloading)
            return;
        if (Time.time < _reloadEndTime)
            return;

        CompleteReload();
    }

    public WeaponFireResult TryFire(Vector2 direction)
    {
        if (!IsEquipped) 
            return WeaponFireResult.NotEquipped;

        if (IsReloading)
            return WeaponFireResult.Reloading;

        if (Time.time < _nextFireTime) 
            return WeaponFireResult.Cooldown;

        if (CurrentAmmo <= 0) 
            return WeaponFireResult.NoAmmo;
        
        int previousAmmo = CurrentAmmo;
        
        CurrentAmmo--;
        
        AmmoChanged?.Invoke(previousAmmo, CurrentAmmo);

        _nextFireTime = Time.time + (1f / _weaponData.FireRate);
        
        _weaponData.WeaponFireStrategy.Execute(this, direction);
        
        WeaponFired?.Invoke();

        return WeaponFireResult.Success;
    }
    
    public void Reload()
    {
        if (!IsEquipped)
            return;
        if (IsReloading)
            return;
        if (CurrentAmmo == WeaponData.MagazineSize)
            return;
        if (ReserveAmmo <= 0)
            return;
        
        IsReloading = true;
        
        ReloadStarted?.Invoke();

        _reloadEndTime = Time.time + _weaponData.ReloadDuration;
    }

    private void CompleteReload()
    {
        int previousAmmo = CurrentAmmo;
        
        int ammoNeeded = WeaponData.MagazineSize - CurrentAmmo;
        int ammoLoaded = Mathf.Min(ammoNeeded, ReserveAmmo);

        CurrentAmmo += ammoLoaded;
        ReserveAmmo -= ammoLoaded;

        IsReloading = false;
        
        AmmoChanged?.Invoke(previousAmmo, CurrentAmmo);
        ReloadCompleted?.Invoke();
    }
    
    public void OnEquipped(WeaponHolder holder)
    {
        _currentHolder = holder;
    }

    public void OnDropped()
    {
        _currentHolder = null;
        IsReloading = false;
    }
}
