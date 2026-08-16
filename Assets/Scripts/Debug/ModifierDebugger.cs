using UnityEngine;

[RequireComponent(typeof(ModifierController))]
public class ModifierDebugger : MonoBehaviour
{
    private ModifierController _modifierController;

    [SerializeField] private float _baseValue = 10f;

    private void Awake()
    {
        _modifierController = GetComponent<ModifierController>();
    }

    [ContextMenu("Test Damage")]
    private void TestDamage()
    {
        float result = _modifierController.CalculateValue(
            _baseValue,
            ModifierStat.Damage);

        Debug.Log($"Base: {_baseValue} | Modified: {result}");
    }
}