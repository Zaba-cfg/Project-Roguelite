using Components.HealthComponents;
using UnityEngine;

namespace Debug
{
    [RequireComponent(typeof(Health))]

    public class HealthDebugger : MonoBehaviour
    {
        private Health _health;
        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.HealthChanged += OnHealthChanged;
            _health.DamageTaken += OnDamageTaken;
            _health.Died += OnDied;
        }
    
        private void OnDisable()
        {
            _health.HealthChanged -= OnHealthChanged;
            _health.DamageTaken -= OnDamageTaken;
            _health.Died -= OnDied;
        }

        private void OnHealthChanged(float previousHealth, float currentHealth)
        {
            UnityEngine.Debug.Log("Health changed: " + previousHealth + " / " + currentHealth);
        }
        private void OnDamageTaken(float damageDealt)
        {
            UnityEngine.Debug.Log("Damage taken: " + damageDealt);
        }
        private void OnDied()
        {
            UnityEngine.Debug.Log("Entity died: " + gameObject.name);
        }
    }
}
