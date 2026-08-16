using UnityEngine;

[CreateAssetMenu(fileName = "StatModifier", menuName = "Modifiers/Stat Modifier")]

public class StatModifier : Modifier
{
    [SerializeField] private ModifierStat _stat;
    [SerializeField] private ModifierOperation _operation;
    [SerializeField] private float _value;
    
    public ModifierStat Stat => _stat;
    public ModifierOperation Operation => _operation;
    public float Value => _value;
}