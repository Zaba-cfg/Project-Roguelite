using UnityEngine;

namespace Components.Base.HealthRelated
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Health))]
    
    public class HealthAudioFeedback : MonoBehaviour
    {
        [SerializeField] private AudioClip _damageClip;
        [SerializeField] private AudioClip _deathClip;

        private AudioSource _audioSource;
        private Health _health;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _health = GetComponent<Health>();
            
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.spatialBlend = 0;
            
            if (_damageClip == null)
                throw new MissingReferenceException($"{name} is missing damage clip.");

            if (_deathClip == null)
                throw new MissingReferenceException($"{name} is missing death clip.");
        }

        private void OnDamageTaken(float damage)
        {
            _audioSource.PlayOneShot(_damageClip);
        }

        private void OnDied()
        {
            _audioSource.PlayOneShot(_deathClip);
        }
        
        private void OnEnable()
        {
            _health.DamageTaken += OnDamageTaken;
            _health.Died += OnDied;
        }

        private void OnDisable()
        {
            _health.DamageTaken -= OnDamageTaken;
            _health.Died -= OnDied;
        }
    }
}