using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponDebugger : MonoBehaviour
{
    private Weapon _weapon;
    private void Awake()
    {
        _weapon = GetComponent<Weapon>();
    }

    private void OnWeaponFired()
    {
        Debug.Log("Weapon Fired!");
    }

    private void OnReloadStarted()
    {
        Debug.Log("Reloading...");
    }

    private void OnReloadCompleted()
    {
        Debug.Log("Weapon Reload Completed!");
    }

    private void OnAmmoChanged(int previous, int newAmmo)
    {
        Debug.Log($"Ammo Changed: {previous}, {newAmmo}");
    }

    private void OnEnable()
    {
        _weapon.AmmoChanged += OnAmmoChanged;
        _weapon.ReloadCompleted += OnReloadCompleted;
        _weapon.ReloadStarted += OnReloadStarted;
        _weapon.WeaponFired += OnWeaponFired;
    }
    
    private void OnDisable()
    {
        _weapon.AmmoChanged -= OnAmmoChanged;
        _weapon.ReloadCompleted -= OnReloadCompleted;
        _weapon.ReloadStarted -= OnReloadStarted;
        _weapon.WeaponFired -= OnWeaponFired;
    }
}