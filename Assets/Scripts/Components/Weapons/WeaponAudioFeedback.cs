using UnityEngine;

namespace Components.Weapons
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Weapon))]
    
    public class WeaponAudioFeedback : MonoBehaviour
    {
        [SerializeField] private AudioClip _fireClip;
        [SerializeField] private AudioClip _reloadStartClip;
        [SerializeField] private AudioClip _reloadCompleteClip;

        private AudioSource _audioSource;
        private Weapon _weapon;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
            _audioSource = GetComponent<AudioSource>();

            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 0;
            
            if (_fireClip == null)
                throw new MissingReferenceException($"{name} is missing fire clip.");

            if (_reloadStartClip == null)
                throw new MissingReferenceException($"{name} is missing reload start clip.");

            if (_reloadCompleteClip == null)
                throw new MissingReferenceException($"{name} is missing reload complete clip.");
        }

        private void OnWeaponFired()
        {
            _audioSource.PlayOneShot(_fireClip);
        }
        
        private void OnReloadStarted()
        {
            _audioSource.PlayOneShot(_reloadStartClip);
        }
        
        private void OnReloadCompleted()
        {
            _audioSource.PlayOneShot(_reloadCompleteClip);
        }
        
        private void OnEnable()
        {
            _weapon.WeaponFired += OnWeaponFired;
            _weapon.ReloadStarted += OnReloadStarted;
            _weapon.ReloadCompleted += OnReloadCompleted;
        }

        private void OnDisable()
        {
            _weapon.WeaponFired -= OnWeaponFired;
            _weapon.ReloadStarted -= OnReloadStarted;
            _weapon.ReloadCompleted -= OnReloadCompleted;
        }
    }
}