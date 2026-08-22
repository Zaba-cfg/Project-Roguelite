using Components.Weapons;
using UnityEngine;

namespace Debug
{
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
            UnityEngine.Debug.Log("Weapon Fired!");
        }

        private void OnReloadStarted()
        {
            UnityEngine.Debug.Log("Reloading...");
        }

        private void OnReloadCompleted()
        {
            UnityEngine.Debug.Log("Weapon Reload Completed!");
        }

        private void OnAmmoChanged(int previous, int newAmmo)
        {
            UnityEngine.Debug.Log($"Ammo Changed: {previous}, {newAmmo}");
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
}