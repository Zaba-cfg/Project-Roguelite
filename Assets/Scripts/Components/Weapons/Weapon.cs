using System;
using UnityEngine;

[RequireComponent(typeof(ModifierInventory))]

public class Weapon : MonoBehaviour
{
    public event Action WeaponFired;
    public event Action<int, int> AmmoChanged;
    public event Action ReloadStarted;
    public event Action ReloadCompleted;
    
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private Transform _muzzle;
    
    private ModifierInventory _modifierInventory;
    private WeaponHolder _currentHolder;
    private float _nextFireTime;
    private float _reloadEndTime;
    
    public bool IsEmpty => CurrentAmmo <= 0 && ReserveAmmo <= 0;
    public WeaponHolder CurrentHolder => _currentHolder;
    public bool IsEquipped => _currentHolder != null;
    public WeaponData WeaponData => _weaponData;
    public float Damage => CalculateModifiedValue(WeaponData.Damage, ModifierStat.Damage);
    public float FireRate => CalculateModifiedValue(WeaponData.FireRate, ModifierStat.FireRate);
    public float ReloadDuration => CalculateModifiedValue(WeaponData.ReloadDuration, ModifierStat.ReloadDuration);
    public Transform Muzzle => _muzzle;
    public GameObject Owner => CurrentHolder?.gameObject;
    
    public int CurrentAmmo { get; private set; }
    public bool IsReloading { get; private set; }
    public int ReserveAmmo { get; private set; }

    private void Awake()
    {
        if (_weaponData == null)
            throw new MissingReferenceException($"{name} is missing WeaponData");

        if (_muzzle == null)
            throw new MissingReferenceException($"{name} is missing Muzzle");
        
        _modifierInventory = GetComponent<ModifierInventory>();
        
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

        _nextFireTime = Time.time + (1f / FireRate);
        
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

        _reloadEndTime = Time.time + ReloadDuration;
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
    
    private float CalculateModifiedValue(float baseValue, ModifierStat stat)
    {
        if (!IsEquipped)
        {
            return _modifierInventory.CalculateValue(baseValue, stat);
        }

        return ModifierCalculator.Calculate(baseValue, stat, _modifierInventory, CurrentHolder.ModifierProvider);
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
