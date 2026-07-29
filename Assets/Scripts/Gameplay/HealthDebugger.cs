using UnityEngine;
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
        Debug.Log("Health changed: " + previousHealth + " / " + currentHealth);
    }
    private void OnDamageTaken(float damageDealt)
    {
        Debug.Log("Damage taken: " + damageDealt);
    }
    private void OnDied()
    {
        Debug.Log("Entity died: " + gameObject.name);
    }
}
