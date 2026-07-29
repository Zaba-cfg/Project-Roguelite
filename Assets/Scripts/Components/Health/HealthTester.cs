using UnityEngine;
[RequireComponent(typeof(Health))]

public class HealthTester : MonoBehaviour
{
    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    [ContextMenu("Take Damage")]
    public void TestDamage()
    {
        _health.TakeDamage(20);
    }
    [ContextMenu("Heal")]
    public void TestHeal()
    {
        _health.Heal(20);
    }
    [ContextMenu("Restore health")]
    public void TestRestoreHealth()
    {
        _health.RestoreFullHealth();
    }
}
